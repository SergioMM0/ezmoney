using RabbitMQ.Client;

namespace RPC.RpcFactory;

public interface IConnectionFactoryProvider {
    ConnectionFactory GetConnectionFactory();
}
