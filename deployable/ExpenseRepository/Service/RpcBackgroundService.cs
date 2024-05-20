using Messages.RPC;
using RPC;
using RPC.RpcFactory;

namespace ExpenseRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Topics _topics;
    public RpcBackgroundService(IServiceScopeFactory scopeFactory, Topics topics) {
        _scopeFactory = scopeFactory;
        _topics = topics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var expenseServiceHandler = scope.ServiceProvider.GetRequiredService<ExpenseRepositoryHandlers>();
            var factoryProvider = scope.ServiceProvider.GetRequiredService<IConnectionFactoryProvider>();
            var rpcServer = new RpcServer(_topics.Topic, expenseServiceHandler, factoryProvider);

            stoppingToken.WaitHandle.WaitOne();
        }
    }
}
