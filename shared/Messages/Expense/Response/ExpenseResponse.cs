namespace Messages.Expense.Response;

/// <summary>
/// Default return object for an expense
/// </summary>
public class ExpenseResponse {
    /// <summary>
    /// Id of the response
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Id of the user that created the expense
    /// </summary>
    public int OwnerId { get; set; }
    
    /// <summary>
    /// Id of the group the expense belongs to
    /// </summary>
    public int GroupId { get; set; }
    
    /// <summary>
    /// Amount of the expense
    /// </summary>
    public double Amount { get; set; }
    
    /// <summary>
    /// Date of the expense
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Small description of the expense
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <summary>
    /// List of Ids of the users that takes part in the expense
    /// </summary>
    public List<int> Participants { get; set; } = null!;
}
