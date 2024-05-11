using Domain;
using Domain.DTO.Group;
using Domain.packages;
using Domain.packages.Interfaces;
using Messages.Group;
using Newtonsoft.Json;
using RPC;

namespace GroupRepository.Service;

public class GroupRepositoryHandlers : IRequestHandler {

    private HandlerRegistry registry;

    private readonly GroupRepositoryService _groupRepositoryService;

    public GroupRepositoryHandlers(GroupRepositoryService groupRepositoryService = null) {
        registry = new HandlerRegistry();
        registry.RegisterHandler(Operation.CreateGroup, HandleCreateGroup);
        registry.RegisterHandler(Operation.GetAllGroups, HandleGetAllGroups);
        registry.RegisterHandler(Operation.GetGroupFromUser, HandleGetGroupFromUser);
        _groupRepositoryService = groupRepositoryService;
    }

    public string ProcessRequest(Operation operation, object data) {
        return registry.HandleRequest(operation, data);
    }
    private string HandleGetGroupFromUser(object data) {
        try {
            var user = JsonConvert.DeserializeObject<User>(data.ToString());
            List<Group> groups = _groupRepositoryService.GetGroupsFromUser(user);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groups) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetAllGroups(object data) {
        try {
            List<Group> groups = new List<Group>();
            groups = _groupRepositoryService.GetAllGroups();
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groups) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleCreateGroup(object data) {
        try {
            var groupDTO = JsonConvert.DeserializeObject<GroupDto>(data.ToString());
            Group groupAdded = _groupRepositoryService.AddGroup(groupDTO);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groupAdded) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    public string HandleRequest(Operation operation, object data) {
        return ProcessRequest(operation, data);
    }


}
