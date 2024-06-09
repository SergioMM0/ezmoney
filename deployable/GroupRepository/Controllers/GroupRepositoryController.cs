using GroupRepository.Service;
using Messages.Group;
using Messages.Group.Request;
using Messages.Group.Response;
using Microsoft.AspNetCore.Mvc;

namespace GroupRepository.Controllers; 
[ApiController]
[Route("[controller]")]
public class GroupRepositoryController : ControllerBase {
    private readonly GroupRepositoryService _groupRepositoryService;

    public GroupRepositoryController(GroupRepositoryService groupRepositoryService) {
        _groupRepositoryService = groupRepositoryService;
    }
    
    [HttpGet("GetGroupsFromUser")]
    public ActionResult<List<GroupResponse>> GetGroupsFromUser(int userId) {
        try {
            Monitoring.Monitoring.Log.Information($"GroupRepository:GetGroupsFromUser:Getting groups from user {userId}");
            return Ok(_groupRepositoryService.GetGroupsFromUser(userId));
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("GetAllGroups")]
    public ActionResult<List<GroupResponse>> GetAllGroups() {
        try {
            return Ok(_groupRepositoryService.GetAllGroups());
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("AddGroup")]
    public ActionResult<GroupResponse> AddGroup(CreateGroupReq request) {
        try {
            var group = _groupRepositoryService.Add(request);
            Monitoring.Monitoring.Log.Information($"Group {group.Name} added");
            return Ok(group);
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("JoinGroup")]
    public ActionResult JoinGroup(JoinGroupReq request) {
        try {
            _groupRepositoryService.JoinGroup(request);
            return Ok();
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("GetGroupById")]
    public ActionResult<GroupResponse> GetGroupById([FromQuery] int groupId) {
        try {
            var groupSearched = new GroupByIdRequest { GroupId = groupId };
            Monitoring.Monitoring.Log.Information($"GroupRepository:GetGroupById:Getting group {groupId}");
            return Ok(_groupRepositoryService.GetGroupById(groupSearched));
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}
