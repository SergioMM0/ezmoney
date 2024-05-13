namespace Messages.Group.Request;

/// <summary>
/// Request object to create a group. This object contains all necessary information to create a group in repository level.
/// </summary>
public class CreateGroupReq {
    /// <summary>
    /// Id of the user that owns the group
    /// </summary>
    public required int OwnerId { get; set; }
    
    /// <summary>
    /// Name of the group
    /// </summary>
    public required string Name { get; set; } = null!;
    
    /// <summary>
    /// Token to join the group
    /// </summary>
    public required string Token { get; set; } = null!;
}
