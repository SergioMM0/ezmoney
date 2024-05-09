using Domain;
using Domain.DTO.Group;
using GroupRepository.Service;

namespace GroupRepository.Repository; 

public class GroupRepository : IGroupRepository{
    private readonly GroupRepositoryContext _context;
    
    public GroupRepository(GroupRepositoryContext context) {
        _context = context;
        CreateDB();
    }
    public List<Group> GetGroupsFromUser(User user) {
        return _context.UserGroupTable
            .Where(ug => ug.UserId == user.Id) 
            .Join(_context.GroupTable,       
                ug => ug.GroupId,            
                g => g.Id,                  
                (ug, g) => g)                 
            .Distinct()                      
            .ToList();                      
        
    }

    public List<Group> GetAllGroups() {
        return _context.GroupTable.ToList();
    }

    public Group AddGroup(GroupDTO group) {
        Group newGroup = new Group {
            Name = group.Name
        };
        
        try {
            _context.GroupTable.Add(newGroup);
            _context.SaveChanges();
            AddUserGroup(group.UserId, newGroup.Id);
            return newGroup;
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the group.", ex);
        }
    }
    public void AddUserGroup(int userId, int groupId) {

        try {
            UserGroup userGroup = new UserGroup {
                UserId = userId,
                GroupId = groupId
            };
            _context.UserGroupTable.Add(userGroup);
            _context.SaveChanges();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    private void CreateDB() {
        Console.WriteLine("Creating database...");
        _context.Database.EnsureCreated();
    }
}
