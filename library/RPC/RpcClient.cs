using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Domain.packages;
public class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
    private readonly string topic;
    private string consumerTag; 

    public RpcClient(string topic)
    {
        var factory = new ConnectionFactory() {
            HostName = "rabbitmq",
            Port = 5672,
            VirtualHost = "/",
            UserName = "guest",
            Password = "guest"
        };
        this.topic = topic;

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        replyQueueName = channel.QueueDeclare().QueueName;

        consumer = new EventingBasicConsumer(channel);
   
        consumerTag = channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true
        );

        consumer.Received += (model, ea) =>
        {
            if (pendingRequests.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                tcs.SetResult(response);
            }
            else
            {
                Console.WriteLine($"Correlation ID mismatch or response too late: {ea.BasicProperties.CorrelationId}");
            }
        };
    }

    public Task<string> CallAsync(Operation operation, object data)
    {
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

        return tcs.Task; 
    }

    public void Close()
    {
        // Cancel the consumer using the stored consumer tag
        if (channel.IsOpen && consumerTag != null)
        {
            channel.BasicCancel(consumerTag);
        }

        // Clear all pending tasks
        foreach (var kvp in pendingRequests)
        {
            kvp.Value.TrySetCanceled();
        }

        // Close the channel and connection
        if (channel.IsOpen)
        {
            channel.Close();
        }
        if (connection.IsOpen)
        {
            connection.Close();
        }
    }
}
