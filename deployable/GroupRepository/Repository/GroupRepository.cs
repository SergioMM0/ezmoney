using Domain;
using Messages.Group.Request;
using Microsoft.EntityFrameworkCore;

namespace GroupRepository.Repository;

public class GroupRepository : IGroupRepository {
    private readonly GroupRepositoryContext _context;

    public GroupRepository(GroupRepositoryContext context) {
        _context = context;
        RecreateDB();
    }

    public List<Group> GetAllGroups() {
        return _context.GroupTable.AsNoTracking().ToList();
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
            throw new ApplicationException("An error occurred while getting the groups.", e);
        }
    }
    
    public Group? GetGroupByToken(string token) {
        var group = _context.GroupTable.AsNoTracking().FirstOrDefault(g => g.Token == token);
        if (group != null) {
            _context.Entry(group).State = EntityState.Detached;
        }
        return group;
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

            // Add UserGroup object to DB to link the user to the group
            AddUserToGroup(group.OwnerId, newGroup.Id);

            // Return the newly created group
            return newGroup;
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the group.", ex);
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
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while adding the user to the group.", e);
        }
    }

    #region DB

    private void RecreateDB() {
        try {
            Console.WriteLine("Creating database...");
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while creating the database.", e);
        }
    }

    #endregion

}
