using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPC.Interfaces;
using RPC.RpcFactory;

namespace RPC;
/// <summary>
/// RPC (Remote Process Call) Server class that receives requests from the Client and sends back a response.
/// </summary>

// RpcServer class implements IDisposable to manage resource cleanup properly
public class RpcServer : IDisposable {
    // Channel for communication with the RabbitMQ broker
    private readonly IModel _channel;
    // The name of the queue "topic" to listen for RPC requests
    private readonly string _queueName;

    // Handler that processes incoming requests and returns responses:
    // This 'IRequestHandler' is an interface that abstracts the processing of various types of operations received through RPC calls.
    // Each operation (like creating, retrieving, or updating data) is associated with a specific method implemented under this interface.
    // The 'requestHandler' object is a concrete implementation of IRequestHandler, which contains logic to decide how to process
    // incoming requests based on the 'Operation' type. It acts as a central dispatcher, which, depending on the operation specified in
    // the request, delegates the handling to the appropriate method. This design helps in organizing the request processing
    // logic systematically, making the server component modular and easy to extend or modify.
    //
    // For instance, in a 'GroupRepositoryHandlers' implementation, different methods such as 'HandleCreateGroup', 'HandleGetAllGroups', 
    // and 'HandleGetGroupFromUser' are registered with a 'HandlerRegistry'. This registry routes the incoming requests to the correct handler
    // based on the operation type. This setup not only decouples the server's request routing logic from the business logic but also
    // simplifies maintenance and scalability of the system by segregating responsibilities.

    private readonly IRequestHandler _requestHandler;
    // Connection to the RabbitMQ broker
    private readonly IConnection _connection;
    // Flag to track if the object has been disposed
    private bool _disposed;


    public RpcServer(string topic, IRequestHandler handler, IConnectionFactoryProvider factoryProvider) {
        // Obtain a connection factory from the provided factory provider
        var factory = factoryProvider.GetConnectionFactory();
        // Create connection and channel using the factory
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        // Set the queue name based on the topic and declare the queue
        _queueName = topic;
        _requestHandler = handler;
        // Set quality of service settings for the channel
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.BasicQos(0, 1, false);
        // Create a consumer that triggers an event when messages are received:
        // The 'EventingBasicConsumer' is a type of consumer provided by RabbitMQ's client library that uses event-driven logic to handle messages.
        // Unlike other types of consumers which might involve continuous polling or other more resource-intensive patterns, the EventingBasicConsumer
        // listens for messages and raises an event whenever a new message is delivered to the queue it's subscribed to.

        // This setup starts by creating a new instance of 'EventingBasicConsumer', passing the channel it should operate on as a parameter.
        // The channel represents the AMQP 0-9-1 channel, and is used to facilitate communication between the consumer and the RabbitMQ broker,
        // including declaring queues, setting QoS, and starting message consumption.

        // Once the consumer is created, an event handler is attached to the 'Received' event. This handler, 'OnReceived', is triggered every time
        // a message is successfully fetched from the queue. The event provides access to the message payload through its arguments,
        // specifically through 'BasicDeliverEventArgs', which includes the message body, routing key, and other metadata.

        // The call to 'channel.BasicConsume' initiates message consumption from the specified 'queueName'. It sets 'autoAck' (automatic acknowledgment)
        // to false, meaning that this server must explicitly acknowledge messages once they are processed successfully. This is crucial for
        // ensuring that messages are not marked as 'delivered' in case of a processing error or server failure, providing a reliable message handling mechanism.
        // The 'consumer' parameter specifies the consumer object that should be used for this subscription.

        // This configuration is important in a server handling potentially multiple or complex requests where operations may not be idempotent,
        // and careful control over message acknowledgment is required to maintain consistency and reliability of the system.

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnReceived!;
        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        Console.WriteLine(" [x] Awaiting RPC requests on queue '{0}'", _queueName);
    }

    // Event handler that processes messages when they are received
    private void OnReceived(object model, BasicDeliverEventArgs ea) {
        // Initialize the response string
        string response = null!;
        // Extract the basic properties and create reply properties
        var props = ea.BasicProperties;
        // Create reply properties with the correlation ID
        var replyProps = _channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;
        // Try to process the request and handle any exceptions
        try {
            // Decode message and deserialize the request
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var request = JsonConvert.DeserializeObject<Request>(message);
            // Handle the request using the injected request handler
            // The request handler processes the request based on the operation type and returns a response
            // The response is then serialized back to JSON format
            // Each Repository Service has its own concretization of IRequestHandler, which contains the logic to handle different operations
            response = _requestHandler.HandleRequest(request!.Operation, request.Data);
        } catch (Exception ex) {
            // Log and serialize error information
            Console.WriteLine("Error processing request: " + ex);
            response = JsonConvert.SerializeObject(new { error = $"Exception: {ex.Message}" });
        } finally {
            // Send the response back to the reply-to address using the correlation ID
            var responseBytes = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            // Acknowledge that the message has been processed thus ending the message processing
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
    // Dispose method to clean up resources
    // when the program is shutting down 
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                if (_channel.IsOpen) {
                    _channel.Close();
                }
                _connection.Close();
            }
            _disposed = true;
        }
    }
}

// Class representing a request message
public class Request {
    public Operation Operation { get; set; }
    public object Data { get; set; } = null!;
}
