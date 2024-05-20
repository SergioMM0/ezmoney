namespace Domain;

public class Group {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Token to join the group. Generated in GroupService
    /// </summary>
    public string Token { get; set; } = null!;
}
