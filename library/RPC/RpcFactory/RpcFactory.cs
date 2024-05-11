using RabbitMQ.Client;

namespace RPC.RpcFactory;

public class RpcFactory : IConnectionFactoryProvider {
    
    public ConnectionFactory GetConnectionFactory() {
        return new ConnectionFactory {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME"),
            Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT")),
            VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VIRTUALHOST"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
        };
    }
}
