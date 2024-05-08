namespace Domain;

public class Expense {
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int GroupId { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
}
