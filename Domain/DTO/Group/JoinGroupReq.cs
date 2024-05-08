namespace Domain.DTO.Group;

/// <summary>
/// DTO for user joining a group
/// </summary>
public class JoinGroupReq {
    public required int UserId { get; set; }
    public required int GroupId { get; set; }
}
