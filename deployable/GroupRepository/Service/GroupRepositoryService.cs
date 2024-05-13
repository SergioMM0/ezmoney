using GroupRepository.Repository;
using Messages.Group;
using Messages.Group.Request;
using Messages.Group.Response;

namespace GroupRepository.Service;

public class GroupRepositoryService {
    private readonly IGroupRepository _groupRepository;

    public GroupRepositoryService(IGroupRepository groupRepository) {
        _groupRepository = groupRepository;
    }

    public List<GroupResponse> GetGroupsFromUser(int userId) {
        return _groupRepository.GetGroupsFromUser(userId)
            .Select(group => new GroupResponse {
                Id = group.Id,
                Name = group.Name,
                Token = group.Token
            })
            .ToList();
    }
    
    public List<GroupResponse> GetAllGroups() {
        return _groupRepository.GetAllGroups()
            .Select(group => new GroupResponse {
                Id = group.Id,
                Name = group.Name,
                Token = group.Token
            })
            .ToList();
    }

    public GroupResponse Add(CreateGroupReq request) {
        var group = _groupRepository.AddGroup(request);
        
        // Maps the BE object to a response object
        return new GroupResponse {
            Id = group.Id,
            Name = group.Name,
            Token = group.Token
        };
    }
    public void JoinGroup(JoinGroupReq request) {
        var group = _groupRepository.GetGroupByToken(request.Token);

        if (group is null) {
            throw new ApplicationException("Group not found");
        }
        
        _groupRepository.AddUserToGroup(request.UserId, group.Id);
    }
}
