using System;
using System.Text;
using Domain.packages;
using Domain.packages.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPC.RpcFactory;

namespace RPC;

public class RpcServer : IDisposable {
    private readonly IModel channel;
    private readonly string queueName;
    private readonly IRequestHandler requestHandler;
    private readonly IConnection connection;
    private bool disposed = false;
    

    public RpcServer(string topic, IRequestHandler handler, IConnectionFactoryProvider factoryProvider) {
        var factory = factoryProvider.GetConnectionFactory();

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queueName = topic;
        requestHandler = handler;

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnReceived;
        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        Console.WriteLine(" [x] Awaiting RPC requests on queue '{0}'", queueName);
    }

    private void OnReceived(object model, BasicDeliverEventArgs ea) {
        string response = null;
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        try {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var request = JsonConvert.DeserializeObject<Request>(message);
            response = requestHandler.HandleRequest(request.Operation, request.Data);
        } catch (Exception ex) {
            Console.WriteLine("Error processing request: " + ex);
            response = JsonConvert.SerializeObject(new { error = $"Exception: {ex.Message}" });
        } finally {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                if (channel.IsOpen) {
                    channel.Close();
                }
                connection.Close();
            }
            disposed = true;
        }
    }
}

public class Request {
    public Operation Operation { get; set; }
    public object Data { get; set; }
}
