using Domain;
using Messages.Group;

namespace GroupRepository.Repository;

public interface IGroupRepository {
    public List<Group> GetGroupsFromUser(User user);
    public List<Group> GetAllGroups();
    public Group AddGroup(CreateGroupReq group);
    public void AddUserGroup(int userId, int groupId);
}
