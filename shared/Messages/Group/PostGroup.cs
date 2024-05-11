namespace Messages.Group;

/// <summary>
/// DTO for creating a new group
/// </summary>
public class PostGroup {
    public required string Name { get; set; }
    public required int UserId { get; set; }
}
