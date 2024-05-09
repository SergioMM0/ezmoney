namespace Domain.DTO.User;

/// <summary>
/// Response object for a user 
/// </summary>
public class UserResponse {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
