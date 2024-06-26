﻿using Messages.Group;
using Messages.Group.Request;
using Messages.Group.Response;
using Newtonsoft.Json;
using RPC;
using RPC.Interfaces;

namespace GroupRepository.Service;

public class GroupRepositoryHandlers : IRequestHandler {

    private readonly HandlerRegistry _registry;

    private readonly GroupRepositoryService _groupRepositoryService;

    public GroupRepositoryHandlers(GroupRepositoryService groupRepositoryService = null!) {
        _registry = new HandlerRegistry();
        _registry.RegisterHandler(Operation.CreateGroup, HandleCreateGroup);
        _registry.RegisterHandler(Operation.GetAllGroups, HandleGetAllGroups);
        _registry.RegisterHandler(Operation.GetGroupsFromUser, HandleGetGroupsFromUser);
        _registry.RegisterHandler(Operation.GetGroupById, HandleGetGroupById);
        _registry.RegisterHandler(Operation.JoinGroup, HandleJoinGroup);
        _groupRepositoryService = groupRepositoryService;
    }

    

    private string ProcessRequest(Operation operation, object data) {
        return _registry.HandleRequest(operation, data);
    }
    
    private string HandleCreateGroup(object data) {
        try {
            var groupDto = JsonConvert.DeserializeObject<CreateGroupReq>(data.ToString()!);
            Monitoring.Monitoring.Log.Debug($"GroupRepositoryHandlers: HandleCreateGroup : CreateGroupReq: {groupDto}");
            var groupAdded = _groupRepositoryService.Add(groupDto!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groupAdded) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }
    private string HandleGetGroupById(object data) {
        try {
            var groupId = JsonConvert.DeserializeObject<GroupByIdRequest>(data.ToString()!);
            var groupSearched = _groupRepositoryService.GetGroupById(groupId);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groupSearched) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }
    private string HandleGetAllGroups(object data) {
        try {
            var groups = _groupRepositoryService.GetAllGroups();
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groups) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }
    
    private string HandleGetGroupsFromUser(object data) {
        try {
            var request = JsonConvert.DeserializeObject<GroupsUserReq>(data.ToString()!);
            var groups = _groupRepositoryService.GetGroupsFromUser(request!.UserId);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(groups) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleJoinGroup(object data) {
        var response = new ApiResponse();
        try {
            var request = JsonConvert.DeserializeObject<JoinGroupReq>(data.ToString()!);
            _groupRepositoryService.JoinGroup(request!);
            response.Success = true;
            response.Data = JsonConvert.SerializeObject("Successfully joined group.");
        } catch (Exception e) {
            response.Success = false;
            response.ErrorMessage = e.Message;
        }
        return JsonConvert.SerializeObject(response);
    }

    public string HandleRequest(Operation operation, object data) {
        return ProcessRequest(operation, data);
    }
}
