using Domain;
using Messages.Group.Request;
using Messages.Group.Response;
using Microsoft.EntityFrameworkCore;

namespace GroupRepository.Repository;

public class GroupRepository : IGroupRepository {
    private readonly GroupRepositoryContext _context;

    public GroupRepository(GroupRepositoryContext context) {
        _context = context;
        RecreateDB();
    }

    public List<Group> GetAllGroups() {
        try {
            return _context.GroupTable.AsNoTracking().ToList();
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while getting the groups: " + e.Message);
            throw new ApplicationException("An error occurred while getting the groups.", e);
        }
       
    }

    public List<Group> GetGroupsFromUser(int userId) {
        try {
            return _context.UserGroupTable
                .Where(ug => ug.UserId == userId)
                .Join(_context.GroupTable,
                    ug => ug.GroupId,
                    g => g.Id,
                    (ug, g) => g)
                .Distinct()
                .AsNoTracking()
                .ToList();
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while getting the groups from User : " + e.Message);
            throw new ApplicationException("An error occurred while getting the groups.", e);
        }
    }
    
    public Group? GetGroupByToken(string token) {
        try {
            var group = _context.GroupTable.AsNoTracking().FirstOrDefault(g => g.Token == token);
            if (group != null) {
                _context.Entry(group).State = EntityState.Detached;
            }
            return group;
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while getting the group by token: " + e.Message);
            throw new ApplicationException("An error occurred while getting the group by token.", e);
        }
        
    }

    public Group AddGroup(CreateGroupReq group) {
        // Map DTO into BE object
        var newGroup = new Group {
            Name = group.Name,
            Token = group.Token
        };
        try {
            // Add group object to the DB
            _context.GroupTable.Add(newGroup);
            _context.SaveChanges();
            Monitoring.Monitoring.Log.Information($"Group {newGroup.Id} {newGroup.Name} {newGroup.Token} added to the database");
            // Add UserGroup object to DB to link the user to the group
            AddUserToGroup(group.OwnerId, newGroup.Id);
            
            // Return the newly created group
            return newGroup;
        } catch (Exception ex) {
            Monitoring.Monitoring.Log.Error("An error occurred while adding the group.", ex.Message);
            throw new ApplicationException("An error occurred while adding the group.", ex);
        }
    }
    public void JoinGroup(int requestUserId, string requestToken) {
        try {
            var group = GetGroupByToken(requestToken);
            
            if (group == null) {
                throw new ApplicationException("Group not found.");
            }
            
            AddUserToGroup(requestUserId, group.Id);
            
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while joining the group: " + e.Message);
            throw new ApplicationException("An error occurred while joining the group.", e);
        }
    }

    public Group GetGroupById(GroupByIdRequest group) {
       Monitoring.Monitoring.Log.Information($"GroupRepository:GetGroupById:Getting group {group.GroupId}");
        try {
            return _context.GroupTable.AsNoTracking().FirstOrDefault(g => g.Id == group.GroupId) ?? throw new ApplicationException("Group not found.");
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while getting the group: " + e.Message);
            throw new ApplicationException("An error occurred while getting the group.", e);
        }
    }

    public void AddUserToGroup(int userId, int groupId) {
        try {
            var userGroup = new UserGroup {
                UserId = userId,
                GroupId = groupId
            };
            _context.UserGroupTable.Add(userGroup);
            _context.SaveChanges();
            Monitoring.Monitoring.Log.Information($"User {userId} added to group {groupId}");
        } catch (Exception e) {
            Monitoring.Monitoring.Log.Error("An error occurred while adding the user to the group: " + e.Message);
            throw new ApplicationException("An error occurred while adding the user to the group.", e);
        }
    }

    #region DB

    private void RecreateDB() {
        try {
            Console.WriteLine("Creating database...");
            //_context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while creating the database.", e);
        }
    }

    #endregion

}
