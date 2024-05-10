namespace Domain.DTO.Group;

/// <summary>
/// Response object for a group
/// </summary>
public class GroupResponse {
    public int Id { get; set; }
    public required string Name { get; set; }
}
