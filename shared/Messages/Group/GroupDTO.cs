namespace Domain.DTO.Group; 


/// <summary>
/// Class for creation of group via the messaging system,
/// I need a json formatted object to send to the server
/// containing the name of the group and the user id
/// </summary>
public class GroupDTO {
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
}
