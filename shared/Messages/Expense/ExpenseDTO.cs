namespace Domain.DTO.Group;

/// <summary>
/// Class for creation of the expense associated to the group via the messaging system,
/// I need a json formatted object to send to the server
/// containing the name of the expense and the user id
/// </summary>
public class ExpenseDTO {
    public int UserId { get; set; }
    public int GroupId { get; set; }
}
