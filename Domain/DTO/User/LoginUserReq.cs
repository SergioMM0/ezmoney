namespace Domain.DTO.User;

/// <summary>
/// DTO for logging in a user
/// </summary>
public class LoginUserReq {
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
