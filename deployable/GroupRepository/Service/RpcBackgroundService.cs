using Messages.RPC;
using RPC;

namespace GroupRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public RpcBackgroundService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var groupServiceHandler = scope.ServiceProvider.GetRequiredService<GroupRepositoryHandlers>();
            var rpcServer = new RpcServer("group_queue", groupServiceHandler);
            stoppingToken.WaitHandle.WaitOne();
            rpcServer.Close();
        }
    }
}
