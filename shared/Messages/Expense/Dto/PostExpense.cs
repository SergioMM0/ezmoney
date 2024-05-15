namespace Messages.Expense.Dto;

/// <summary>
/// DTO for creating a new expense that comes from client side.
/// </summary>
public class PostExpense {
    /// <summary>
    /// Id of the user that created the expense.
    /// </summary>
    public required int OwnerId { get; set; }
    
    /// <summary>
    /// Id of the group that the expense belongs to.
    /// </summary>
    public required int GroupId { get; set; }
    
    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public required double Amount { get; set; }
    
    /// <summary>
    /// Date of the expense.
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Small description of the expense.
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Id of the users that participated in the expense.
    /// </summary>
    public List<int> Participants { get; set; } = null!;
}
