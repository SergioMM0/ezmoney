using Messages.RPC;
using OpenTelemetry.Trace;
using RPC;
using RPC.RpcFactory;

namespace ExpenseRepository.Service;

public class RpcBackgroundService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Topics _topics;
    private readonly Tracer? _tracer;
    public RpcBackgroundService(IServiceScopeFactory scopeFactory, Topics topics, Tracer? tracer = null) {
        _scopeFactory = scopeFactory;
        _topics = topics;
        _tracer = tracer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using (var scope = _scopeFactory.CreateScope()) {
            var expenseServiceHandler = scope.ServiceProvider.GetRequiredService<ExpenseRepositoryHandlers>();
            var factoryProvider = scope.ServiceProvider.GetRequiredService<IConnectionFactoryProvider>();
            var rpcServer = new RpcServer(_topics.Topic, expenseServiceHandler, factoryProvider, _tracer);

            stoppingToken.WaitHandle.WaitOne();
        }
    }
}
