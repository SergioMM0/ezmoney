using Newtonsoft.Json;

namespace RPC.Interfaces;

public class HandlerRegistry : IRequestHandler {
    private readonly Dictionary<Operation, Func<object, string>> _handlers = new Dictionary<Operation, Func<object, string>>();

    public void RegisterHandler(Operation operation, Func<object, string> handler) {
        _handlers[operation] = handler;
    }

    public string HandleRequest(Operation operation, object data) {
        if (_handlers.TryGetValue(operation, out var handler)) {
            return handler(data);
        }
        return JsonConvert.SerializeObject(new { error = $"No handler registered for {operation}" });
    }
}
