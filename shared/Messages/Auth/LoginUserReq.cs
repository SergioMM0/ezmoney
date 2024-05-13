namespace Messages.User;

/// <summary>
/// DTO for logging in a user
/// </summary>
public class LoginUserReq {
    public required string PhoneNumber { get; set; }
}
