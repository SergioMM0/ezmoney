using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPC.RpcFactory;

namespace RPC;

/// <summary>
/// RPC (Remote Process Call) Client class that sends requests to the server and receives responses.
/// </summary>

// RpcClient class implements IDisposable to manage resource cleanup properly when the program shuts down.
// The RpcClient class is responsible for sending requests to the server and receiving responses asynchronously.
// It uses RabbitMQ to establish a connection to the server and communicate over a channel.
// The RpcClient class is designed to handle multiple outbound requests and inbound responses concurrently.
// It uses a dictionary to store pending requests and their associated TaskCompletionSource objects.
// This allows the RpcClient to match responses to their corresponding requests and complete the associated tasks when responses arrive.
// The RpcClient class is used by the Controller to send requests to the server and receive responses asynchronously.
public class RpcClient : IDisposable {
    // Object containing the connection to the RabbitMQ server
    // It is an interface that can allow for mocking in unit tests
    // It is also a dependency that is injected into the class
    private readonly IConnection _connection;

    // Channel to communicate with RabbitMQ:
    // The channel is a fundamental construct in RabbitMQ, representing a virtual connection inside 
    // a physical TCP connection. It's used for all operations involving communication with the RabbitMQ 
    // server, such as publishing messages, subscribing to queues, and performing transactional operations.
    // Given its role in our application, the channel is a critical component of the RpcClient-RpcServer architecture. 
    // Using a single persistent channel optimizes 
    // resource usage , while still maintaining high scalability
    // in client-server interactions.


    private readonly IModel _channel;

    // Name of the queue for replies:
    // This variable stores the name of the queue specifically dedicated to receiving responses in RPC interactions.
    // In HTTP (Synchronous or Asynchronous) setups, each client typically sends requests to a server which processes them and sends back responses. 
    // The 'replyQueueName' is dynamically generated when the channel declares a new queue. This queue is often exclusive and 
    // auto-deleted when the client disconnects. It serves as a temporary mailbox for responses to requests made by this specific client.
    // Using a unique reply queue for each client or session helps ensure that messages are securely and accurately directed back to the requester,
    // supporting concurrency and direct response handling without message loss or misrouting.
    // Because the reply queue is exclusive, it is automatically deleted when the client disconnects, preventing resource leaks and ensuring
    // that the queue is only used for the duration of the client's connection to the server.
    // The reply queue is a critical component of the RPC architecture, enabling the client to receive responses to its requests in a timely and accurate manner.
    // it is also declared only once when the RpcClient is instantiated, and is used throughout the client's lifecycle to receive responses from the server.
    // this significantly reduces the overhead of creating and managing multiple queues for each request-response cycle, thus making the responses 
    // "faster".

    private readonly string _replyQueueName;

    // EventingBasicConsumer is a RabbitMQ consumer that uses event-based message handling.
    // This consumer is specifically used to asynchronously receive messages from the RabbitMQ broker.
    // When the RpcClient sends a request to the server, it expects a response. The responses are delivered to
    // the queue specified by 'replyQueueName' (which is like a temporary queue between the Controller and the Rpc Server), and this consumer is subscribed to that queue.
    // 
    // The 'consumer' listens for messages arriving in the reply queue and triggers the Received event each time a new
    // message is delivered. The event handler associated with the Received event (set up in the RpcClient constructor)
    // is responsible for processing these messages. It retrieves the correlation ID from the incoming message's properties,
    // checks if it matches any pending request stored in 'pendingRequests', and if a match is found, completes the associated
    // TaskCompletionSource with the message payload.
    // Pending requests represent asynchronous operations that are awaiting responses from the RabbitMQ server.
    // In the RpcClient, 'pendingRequests' is implemented as a ConcurrentDictionary where each entry maps a unique correlation ID to a TaskCompletionSource<string>.
    // The correlation ID is a unique identifier for each request sent to the server, ensuring that responses can be matched correctly to their corresponding requests.

    // The TaskCompletionSource<string> associated with each correlation ID represent a Task waiting for the result of an asynchronous operation. When a request is sent to the server via RabbitMQ,
    // a new TaskCompletionSource<string> is created and added to 'pendingRequests' with the correlation ID of the request. This TaskCompletionSource is then used to "catch" the result of a request.

    // When a response arrives from the server, the RpcClient's consumer event handler search the correlation ID of the response in the the 'pendingRequests' dictionary.
    // If a matching entry is found, the TaskCompletionSource<string> is completed and the response data (Line: var response = Encoding.UTF8.GetString(ea.Body.ToArray());,)
    // is sent back to the Controller.
    // This allows the original caller (who is be awaiting the Task returned by CallAsync) to receive the response data asynchronously once it's available.

    // This mechanism is crucial in this case and is the cornerstone of our Asynchronous RabbitMQ communication . It allows the RpcClient to handle multiple outbound requests and inbound responses concurrently,
    // via the Dictionary oncurrentDictionary<string, TaskCompletionSource<string>> pendingRequests.
    // This mechanism enables asynchronous, non-blocking RPC-style communication
    // where the RpcClient can continue with other tasks while waiting for responses to its requests.
    //
    // This approach ensures efficient handling of responses without blocking the thread.

    private readonly EventingBasicConsumer _consumer;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests =
        new ConcurrentDictionary<string, TaskCompletionSource<string>>();

    // The topic is the routing key used to send messages to the server.
    private readonly string _topic;

    // Flag to track whether the object has been disposed.
    private bool _disposed;
    
    //Propagating the tracing context (optional)
    private readonly Tracer? _tracer;

    public RpcClient(string topic, IConnectionFactoryProvider factoryProvider, Tracer? tracer = null) {
        // Retrieving the connection details from the factory provider.
        var factory = factoryProvider.GetConnectionFactory();

        this._topic = topic;

        _connection = factory.CreateConnection();
        // Creating a channel for communication with RabbitMQ.
        // The channel is a virtual connection inside a physical TCP connection.
        // And will persist throughout the lifetime of the RpcClient object.
        _channel = _connection.CreateModel();
        // Declaring a queue for receiving responses from the server.
        // The queue is exclusive and auto-deleted when the client disconnects.
        // The replyQueueName is a temporary queue that is used to receive responses from the server.
        // The replyQueueName is unique to each client and is used to receive responses to requests made by that client.
        // It will persist until the Client is Disposed.
        _replyQueueName = _channel.QueueDeclare().QueueName;
        // Creating a consumer to listen for messages arriving in the reply queue.
        // The consumer is an EventingBasicConsumer that triggers the Received event each time a new message is delivered.
        _consumer = new EventingBasicConsumer(_channel);
        // Subscribing the consumer to the reply queue to start receiving messages.
        // The consumer listens for messages arriving in the reply queue and triggers the Received event each time a new message is delivered.
        _channel.BasicConsume(
            consumer: _consumer,
            queue: _replyQueueName,
            autoAck: true
        );
        
        _tracer = tracer; // Optional
        // Event handler for processing incoming messages.
        // The event handler processes incoming messages by extracting the correlation ID from the message properties,
        // matching it to a pending request, and completing the associated TaskCompletionSource with the message payload.
        // This mechanism ensures that responses are correctly matched to their corresponding requests.
        _consumer.Received += (_, ea) => {
            if (_pendingRequests.TryRemove(ea.BasicProperties.CorrelationId, out var tcs)) {
                if (tracer != null) {
                    var propagatorExtract = new TraceContextPropagator();
                    var parentContext = propagatorExtract.Extract(default, ea.BasicProperties, (req, key) => {
                        return new List<string>(new[] {
                            req.Headers.ContainsKey(key) ? req.Headers[key].ToString() : String.Empty
                        });
                    });
                    Baggage.Current = parentContext.Baggage;
                    
                    using var consumerActivity = _tracer.StartActiveSpan("RpcClient - ConsumerActivity");
                    using var activity = _tracer.StartActiveSpan("RpcClient - Received");
                }
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                tcs.SetResult(response);
            } else {
                Monitoring.Monitoring.Log.Error($"Correlation ID mismatch or response too late: {ea.BasicProperties.CorrelationId}");
                Console.WriteLine($"Correlation ID mismatch or response too late: {ea.BasicProperties.CorrelationId}");
            }
        };
        
    }
    public Task<string> CallAsync(Operation operation, object data) {
        return CallAsync(operation, data, TimeSpan.FromSeconds(30)); // Default timeout of 30 seconds
    }
    public async Task<string> CallAsync(Operation operation, object data, TimeSpan timeout) {
        // Generating a unique correlation ID for the request.
        var correlationId = Guid.NewGuid().ToString();
        // Creating basic properties for the MESSAGE.
        // The basic properties include the correlation ID and the reply queue name.
        var props = _channel.CreateBasicProperties();
        props.CorrelationId = correlationId;
        // The replyTo property is set to the reply queue name.
        props.ReplyTo = _replyQueueName;
        // Propagating the tracing context (optional)
        if (_tracer != null)
        {
            using var activity = _tracer.StartActiveSpan("RpcClient:SendAsync");
            props.Headers = new Dictionary<string, object>();
            var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
            var propagationContext = new PropagationContext(activityContext, Baggage.Current);
            var propagator = new TraceContextPropagator();
            propagator.Inject(propagationContext, props, (msg, key, value) => { msg.Headers.Add(key, value); });
        }
        // Creating a request object with the operation and data.
        // data is the object that has been serialized by the controller it contains the necessary
        // information to perform the operation.
        var request = new { Operation = operation, Data = data };
        var message = JsonConvert.SerializeObject(request);
        // Converting the message to a byte array.
        // The message is serialized to a byte array using UTF-8 encoding before being sent to the server.
        //Messages sent to RabbitMQ must be in a format that can be reliably transmitted over the network and
        //understood by the message broker and any receiving clients. In the case of RabbitMQ, which is fundamentally agnostic
        //about the content of the messages, data must be transformed into a binary format—hence the byte array
    
        var messageBytes = Encoding.UTF8.GetBytes(message);
        // Adding the correlation ID and the TaskCompletionSource to the pending requests dictionary.
        var tcs = new TaskCompletionSource<string>();
        _pendingRequests[correlationId] = tcs;
        // Publishing the message to the server.
        _channel.BasicPublish(
            exchange: "",
            routingKey: _topic,
            basicProperties: props,
            body: messageBytes);
        
        var timeoutTask = Task.Delay(timeout).ContinueWith(_ => "TimeOut");
        var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

        if (completedTask == timeoutTask) {
            _pendingRequests.TryRemove(correlationId, out _);
            Monitoring.Monitoring.Log.Error("RpcClient::CallAsync::Request timed out.");
            throw new RpcTimeoutException("RPC request timed out.");
        }

        // Returning the Task associated with the request.
       
        // Deserializing the response from the server.
        // The response from the server is deserialized from JSON to an ApiResponse object.
        // The ApiResponse object contains the success status, error message, and data returned by the server.
        // If the response indicates success, the data is returned to the caller.
        // If the response indicates an error, an ApplicationException is thrown with the error message.
        // This mechanism ensures that the caller receives the correct response data or an Exception from the RPC server.
        // as normally the repository will return only a string.
    
            
        var result = await tcs.Task;
        var response = JsonConvert.DeserializeObject<ApiResponse>(result);
        if (!response.Success) {
            Monitoring.Monitoring.Log.Error($"RpcClient::CallAsync::Error: {response.ErrorMessage}");
            throw new ApplicationException(response.ErrorMessage);
        }

        return response.Data;
    }
    
    // Disposing of the RpcClient object.
    // The Dispose method is called to release resources used by the RpcClient object.
    // This method ensures that the connection to the RabbitMQ server is closed and that the channel is disposed of properly.
    public void Dispose() {
        if (!_disposed) {
            if (_channel.IsOpen) {
                _channel.Close();
            }
            if (_connection.IsOpen) {
                _connection.Close();
            }
            _disposed = true;
        }
    }
}
