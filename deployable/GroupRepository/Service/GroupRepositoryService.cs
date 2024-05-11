using Domain;
using Domain.DTO.Group;
using GroupRepository.Repository;
using Messages.Group;

namespace GroupRepository.Service;

public class GroupRepositoryService {
    private readonly IGroupRepository _groupRepository;

    public GroupRepositoryService(IGroupRepository groupRepository) {
        _groupRepository = groupRepository;
    }

    public List<Group> GetGroupsFromUser(User user) {
        return _groupRepository.GetGroupsFromUser(user);
    }

    public List<Group> GetAllGroups() {
        return _groupRepository.GetAllGroups();
    }

    public Group AddGroup(GroupDto group) {
        return _groupRepository.AddGroup(group);
    }
}
