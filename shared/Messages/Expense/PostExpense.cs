namespace Messages.Expense;

public class PostExpense {
    public required int OwnerId { get; set; }
    public required int GroupId { get; set; }
    public required double Amount { get; set; }
    public DateTime Date { get; set; }
    public required string Description { get; set; }
    public List<int> Participants { get; set; } = null!;
}
