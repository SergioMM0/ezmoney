namespace RPC; 

public class RpcTimeoutException : Exception
{
    public RpcTimeoutException(string message) : base(message) { }
}
