namespace Messages.User.Request;

/// <summary>
/// Request for getting a user by phone number
/// </summary>
public class GetUserByPhone {
    /// <summary>
    /// Phone number of the user to retrieve
    /// </summary>
    public required string PhoneNumber { get; set; }
}
