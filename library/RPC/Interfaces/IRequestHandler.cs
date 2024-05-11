namespace RPC.Interfaces;

public interface IRequestHandler {
    string HandleRequest(Operation operation, object data);
}
