using RPC;

namespace Domain.packages.Interfaces;

public interface IRequestHandler {
    string HandleRequest(Operation operation, object data);
}
