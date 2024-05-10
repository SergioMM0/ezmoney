using Messages.RPC;
using RPC;
using RPC.RpcFactory;

namespace UserRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Topics _topics;
    public RpcBackgroundService(IServiceScopeFactory scopeFactory, Topics topics) {
        _scopeFactory = scopeFactory;
        _topics = topics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var userServiceHandler = scope.ServiceProvider.GetRequiredService<UserRepositoryHandlers>();
            var factoryProvider = scope.ServiceProvider.GetRequiredService<IConnectionFactoryProvider>();
            var rpcServer = new RpcServer(_topics.Topic, userServiceHandler, factoryProvider);

            stoppingToken.WaitHandle.WaitOne();
        }
    }
}
