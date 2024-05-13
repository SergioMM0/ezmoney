namespace Messages.Group;

/// <summary>
/// Response object for a group
/// </summary>
public class GroupResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Token { get; set; }
}
