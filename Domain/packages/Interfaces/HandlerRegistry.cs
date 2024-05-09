using Newtonsoft.Json;

namespace Domain.packages.Interfaces; 

public class HandlerRegistry : IRequestHandler
{
    private readonly Dictionary<Operation, Func<object, string>> handlers = new Dictionary<Operation, Func<object, string>>();

    public void RegisterHandler(Operation operation, Func<object, string> handler)
    {
        handlers[operation] = handler;
    }

    public string HandleRequest(Operation operation, object data)
    {
        if (handlers.TryGetValue(operation, out var handler))
        {
            return handler(data);
        }
        return JsonConvert.SerializeObject(new { error = $"No handler registered for {operation}" });
    }
}
