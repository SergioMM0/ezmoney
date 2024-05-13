namespace Messages.User;

/// <summary>
/// DTO for creating a user
/// </summary>
public class CreateUserReq {
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
