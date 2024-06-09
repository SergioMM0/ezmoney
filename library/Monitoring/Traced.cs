namespace Monitoring;

public class Traced<T> {
    public Dictionary<string, string> Headers { get; set; } = new();
    public T Data { get; set; }
}
