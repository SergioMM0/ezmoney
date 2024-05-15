namespace Messages.Expense.Request;

/// <summary>
/// Object that represents the request to create an expense for RPC
/// </summary>
public class CreateExpenseReq {
    /// <summary>
    /// Id of the user that created the expense
    /// </summary>
    public int OwnerId { get; set; }
    
    /// <summary>
    /// Group id of the group the expense belongs to
    /// </summary>
    public int GroupId { get; set; }
    
    /// <summary>
    /// Amount of the expense
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Small description of the expense
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// List of the Ids of the user that participated in the expense
    /// </summary>
    public List<int> Participants { get; set; } = null!;
    
    /// <summary>
    /// Date of the expense
    /// </summary>
    public DateTime Date { get; set; }
}
