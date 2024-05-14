using Domain;
using Messages.Group.Request;

namespace GroupRepository.Repository;

public interface IGroupRepository {
    List<Group> GetAllGroups();
    List<Group> GetGroupsFromUser(int userId);
    Group? GetGroupByToken(string token);
    Group AddGroup(CreateGroupReq group);
    void JoinGroup(int requestUserId, string requestToken);
}
