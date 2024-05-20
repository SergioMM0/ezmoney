namespace Messages.Expense.Request;

/// <summary>
/// Object to request all expenses from a group
/// </summary>
public class GetExpensesReq {
    
    /// <summary>
    /// The Id of the grouo
    /// </summary>
    public int GroupId { get; set; }
}
