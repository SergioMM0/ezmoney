namespace Messages.Group.Response;

/// <summary>
/// Response object for Group BE
/// </summary>
public class GroupResponse {
    /// <summary>
    /// Id of the <c>Group</c>
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Name of the <c>Group</c>
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Token of the <c>Group</c>
    /// </summary>
    public required string Token { get; set; }
}
