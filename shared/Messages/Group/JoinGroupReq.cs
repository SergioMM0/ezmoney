namespace Messages.Group;

/// <summary>
/// DTO for user joining a group
/// </summary>
public class JoinGroupReq {
    /// <summary>
    /// The id of the user that wants to join the group
    /// </summary>
    public required int UserId { get; set; }
    
    /// <summary>
    /// The token of the group that the user wants to join
    /// </summary>
    public required string Token { get; set; }
}
