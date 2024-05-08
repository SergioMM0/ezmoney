namespace Domain.packages; 
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;
    private readonly string topic;

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
        props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        consumer.Received += (model, ea) =>
        {
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                respQueue.Add(response);
            }
        };

        channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);
    }

    public T Call<T>(object request)
    {
        var message = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: topic,
            basicProperties: props,
            body: messageBytes);

        var response = respQueue.Take(); // Blocks here until a message is added to the collection
        return JsonConvert.DeserializeObject<T>(response); // Deserialize the JSON back into the object
    }

    public void Close()
    {
        connection.Close();
    }
}
