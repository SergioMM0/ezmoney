﻿using Domain;
using Messages.Group;

namespace GroupRepository.Repository;

public class GroupRepository : IGroupRepository {
    private readonly GroupRepositoryContext _context;

    public GroupRepository(GroupRepositoryContext context) {
        _context = context;
        CreateDB();
    }
    public List<Group> GetGroupsFromUser(User user) {
        try {
            return _context.UserGroupTable
                .Where(ug => ug.UserId == user.Id)
                .Join(_context.GroupTable,
                    ug => ug.GroupId,
                    g => g.Id,
                    (ug, g) => g)
                .Distinct()
                .ToList();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while getting the groups.", e);
        }
    }

    public List<Group> GetAllGroups() {
        return _context.GroupTable.ToList();
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
            AddUserGroup(group.OwnerId, newGroup.Id);
            
            // Return the newly created group
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
            throw new ApplicationException("An error occurred while adding the user to the group.", e);
        }
    }

    #region DB

    private void CreateDB() {
        try {
            Console.WriteLine("Creating database...");
            _context.Database.EnsureCreated();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while creating the database.", e);
        }
    }

    #endregion
}
