namespace Messages.Auth;

/// <summary>
/// DTO for logging in a user
/// </summary>
public class LoginUserReq {
    
    /// <summary>
    /// Phone number of the user
    /// </summary>
    public required string PhoneNumber { get; set; }
}
