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
            return Ok(_groupRepositoryService.Add(request));
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
    public ActionResult<GroupResponse> GetGroupById(GroupByIdRequest groupSearched) {
        try {
            return Ok(_groupRepositoryService.GetGroupById(groupSearched));
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}
