using RabbitMQ.Client;

namespace RPC.RpcFactory; 

public class RpcFactory : IConnectionFactoryProvider
{
    public ConnectionFactory GetConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            VirtualHost = "/",
            UserName = "guest",
            Password = "guest"
        };
    }
}
