namespace RPC;

public class ApiResponse {

    public bool Success { get; set; }

    public string Data { get; set; } = null!;

    public string ErrorMessage { get; set; } = null!;
}
