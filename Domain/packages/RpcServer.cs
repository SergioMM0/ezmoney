namespace Domain.packages; 

using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class RpcServer<T>
{
    private readonly IModel channel;
    private readonly string queueName;

    public RpcServer(string topic)
    {
        var factory = new ConnectionFactory() {
            HostName = "rabbitmq",
            Port = 5672,
            VirtualHost = "/",
            UserName = "guest",
            Password = "guest"
        };

        var connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queueName = topic;

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnReceived;

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        Console.WriteLine(" [x] Awaiting RPC requests on queue '{0}'", queueName);
    }

    private void OnReceived(object model, BasicDeliverEventArgs ea)
    {
        string response = null;
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        try
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            T requestObject = JsonConvert.DeserializeObject<T>(message);
            Console.WriteLine("Received a request.");

            // Process the received object and prepare a response
            response = JsonConvert.SerializeObject(ProcessRequest(requestObject));
        }
        catch (Exception e)
        {
            Console.WriteLine(" [.] Exception: {0}", e.Message);
            response = JsonConvert.SerializeObject(new { error = e.Message });
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }

    // Example processing method, can be adjusted as needed
    private T ProcessRequest(T input)
    {
        // Mock processing, here you can add logic to modify the object or create a new response object of the same type
        return input; // Simply returning the input for demonstration
    }

    public void Close()
    {
        channel.Close();
    }
}
