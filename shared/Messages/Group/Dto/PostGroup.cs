namespace Messages.Group;

/// <summary>
/// Clients request to create a group
/// </summary>
public class PostGroup {
    /// <summary>
    /// Id of the user that owns the <c>Group</c>
    /// </summary>
    public required int OwnerId { get; set; }
    
    /// <summary>
    /// Name of the <c>Group</c>
    /// </summary>
    public required string Name { get; set; }
}
