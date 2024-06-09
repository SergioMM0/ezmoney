using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;

namespace RPC.Interfaces;

public class HandlerRegistry : IRequestHandler {
    private readonly Dictionary<Operation, Func<object, string>> _handlers = new Dictionary<Operation, Func<object, string>>();
    private readonly Tracer _tracer;
    
    public HandlerRegistry(Tracer tracer = null) {
        _tracer = tracer;
    }

    public void RegisterHandler(Operation operation, Func<object, string> handler) {
        _handlers[operation] = handler;
    }

    public string HandleRequest(Operation operation, object data) {
        using var activity = _tracer.StartActiveSpan("HandleRequest - HandlerRegistry");
        if (_handlers.TryGetValue(operation, out var handler)) {
            return handler(data);
        }
        return JsonConvert.SerializeObject(new { error = $"No handler registered for {operation}" });
    }
}
