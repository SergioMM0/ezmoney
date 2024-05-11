namespace Messages.Group;

/// <summary>
/// DTO for request to get all members from a group
/// </summary>
public class GroupMembersReq {
    public required int GroupId { get; set; }
}
