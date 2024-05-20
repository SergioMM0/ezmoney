using Messages.RPC;
using RPC;
using RPC.RpcFactory;

namespace GroupRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Topics _topics;

    public RpcBackgroundService(IServiceScopeFactory scopeFactory, Topics topics) {
        _scopeFactory = scopeFactory;
        _topics = topics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using var scope = _scopeFactory.CreateScope();
        var groupServiceHandler = scope.ServiceProvider.GetRequiredService<GroupRepositoryHandlers>();
        var factoryProvider = scope.ServiceProvider.GetRequiredService<IConnectionFactoryProvider>();
        var rpcServer = new RpcServer(_topics.Topic, groupServiceHandler, factoryProvider);
        stoppingToken.WaitHandle.WaitOne();
    }
}
