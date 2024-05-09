namespace Domain.DTO.Group; 

/// <summary>
/// Class for creation of the expense associated to the group via the messaging system,
/// I need a json formatted object to send to the server
/// containing the name of the expense and the user id
/// </summary>
public class ExpenseDTO {
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int GroupId { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public List<int> Participants { get; set; } = null!;
}
