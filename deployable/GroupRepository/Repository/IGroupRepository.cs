using Domain;
using Domain.DTO.Group;

namespace GroupRepository.Repository;

public interface IGroupRepository {
    public List<Group> GetGroupsFromUser(User user);
    public List<Group> GetAllGroups();
    public Group AddGroup(GroupDTO group);
    public void AddUserGroup(int userId, int groupId);
}
