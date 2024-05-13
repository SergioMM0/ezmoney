namespace Messages.Group.Request;

/// <summary>
/// DTO for request to get all members from a group
/// </summary>
public class GroupsUserReq {
    /// <summary>
    /// Id of the user to get all groups from
    /// </summary>
    public required int UserId { get; set; }
}
