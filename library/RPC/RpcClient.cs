using System;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using RPC.RpcFactory;

namespace Domain.packages;
public class RpcClient : IDisposable {
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
    private readonly string topic;
    private bool disposed = false;

    public RpcClient(string topic, IConnectionFactoryProvider factoryProvider) {
        var factory = factoryProvider.GetConnectionFactory();
        
        this.topic = topic;

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        replyQueueName = channel.QueueDeclare().QueueName;

        consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true
        );

        consumer.Received += (model, ea) => {
            if (pendingRequests.TryRemove(ea.BasicProperties.CorrelationId, out var tcs)) {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                tcs.SetResult(response);
            } else {
                Console.WriteLine($"Correlation ID mismatch or response too late: {ea.BasicProperties.CorrelationId}");
            }
        };
    }

    public Task<string> CallAsync(Operation operation, object data) {
        var correlationId = Guid.NewGuid().ToString();
        var props = channel.CreateBasicProperties();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        var request = new { Operation = operation, Data = data };
        var message = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        var tcs = new TaskCompletionSource<string>();
        pendingRequests[correlationId] = tcs;
        channel.BasicPublish(
            exchange: "",
            routingKey: topic,
            basicProperties: props,
            body: messageBytes);

        return tcs.Task.ContinueWith(task => {
            if (task.IsFaulted) {
                throw task.Exception ?? new Exception("Task failed without an exception.");
            }

            var response = JsonConvert.DeserializeObject<ApiResponse>(task.Result);
            if (!response.Success) {
                throw new ApplicationException(response.ErrorMessage);
            }

            return response.Data;
        });
    }

    public void Dispose() {
        if (!disposed) {
            if (channel.IsOpen) {
                channel.Close();
            }
            if (connection.IsOpen) {
                connection.Close();
            }
            disposed = true;
        }
    }
}
