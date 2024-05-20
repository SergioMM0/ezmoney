namespace Messages.User.Dto;

/// <summary>
/// Clients dto to create a user
/// </summary>
public class PostUser {
    /// <summary>
    /// Name of the user
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Phone number of the user
    /// </summary>
    public required string PhoneNumber { get; set; }
}
