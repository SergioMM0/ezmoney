using Domain.packages;

namespace UserRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public RpcBackgroundService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var userServiceHandler = scope.ServiceProvider.GetRequiredService<UserRepositoryHandlers>();
            var rpcServer = new RpcServer("user_queue", userServiceHandler);
            stoppingToken.WaitHandle.WaitOne();
            rpcServer.Close();
        }
    }
}
