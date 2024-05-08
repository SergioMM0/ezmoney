namespace Domain.DTO.Group;

/// <summary>
/// DTO for user joining a group
/// </summary>
public class JoinGroupReq {
    public required int UserId { get; set; }
    public required int Token { get; set; } // Join token of the group
}
