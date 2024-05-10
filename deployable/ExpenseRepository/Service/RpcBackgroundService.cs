using Domain.packages;
namespace ExpenseRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;

    public RpcBackgroundService(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var expenseServiceHandler = scope.ServiceProvider.GetRequiredService<ExpenseRepositoryHandlers>();
            var rpcServer = new RpcServer("expense_queue", expenseServiceHandler);
            stoppingToken.WaitHandle.WaitOne();
            rpcServer.Close();
        }
    }
}
