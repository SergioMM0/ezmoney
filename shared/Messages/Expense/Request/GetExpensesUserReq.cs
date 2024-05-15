namespace Messages.Expense.Request;

/// <summary>
/// Object to request the expenses from a user in a group
/// </summary>
public class GetExpensesUserReq {
    /// <summary>
    /// The id of the user
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// The id of the group
    /// </summary>
    public int GroupId { get; set; }
}
