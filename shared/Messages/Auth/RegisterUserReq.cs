namespace Messages.Auth;

/// <summary>
/// DTO for registering a user
/// </summary>
public class RegisterUserReq {
    /// <summary>
    /// Name of the user to register
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// PhoneNumber of the user to register
    /// </summary>
    public required string PhoneNumber { get; set; }
}
