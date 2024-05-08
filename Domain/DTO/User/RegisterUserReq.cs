namespace Domain.DTO.User;

/// <summary>
/// DTO for registering a user
/// </summary>
public class RegisterUserReq {
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
