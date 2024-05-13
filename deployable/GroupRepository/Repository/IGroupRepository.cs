using Domain;
using Messages.Group.Request;

namespace GroupRepository.Repository;

public interface IGroupRepository {
    public List<Group> GetAllGroups();
    public List<Group> GetGroupsFromUser(int userId);
    public Group AddGroup(CreateGroupReq group);
}
