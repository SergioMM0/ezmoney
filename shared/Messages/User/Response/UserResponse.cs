namespace Messages.User.Response;

/// <summary>
/// Response object for a user 
/// </summary>
public class UserResponse {
    /// <summary>
    /// Id of the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the user
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Phone number of the user
    /// </summary>
    public required string PhoneNumber { get; set; }
}
