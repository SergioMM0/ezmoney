using Domain;
using Domain.DTO.Group;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroupService.Controller;

[ApiController]
[Route("group")]
public class GroupController : ControllerBase {

    /* NOT IN THE INITIAL REQUIREMENTS, MIGHT BE USED LATER
    [HttpGet("/{groupId}/members")]
    public IActionResult AllMembersFromGroup([FromRoute] int groupId) {
        if(groupId == 1) {
            return Ok("yo mama says hi");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
    }
    */
    private readonly RpcClient _rpcClient;
    
    public GroupController() {
        _rpcClient = new RpcClient("group_queue");
    }
    
    [HttpGet("{userId}/groups")] //group/1/groups
    public async Task<ActionResult<List<GroupResponse>>> GetAllGroupsOfUser([FromRoute] int userId) {
        try {
            Console.WriteLine("Sending a request to create a user...");
            var response = await _rpcClient.CallAsync(Operation.GetGroupFromUser, new User { Id  = userId, Name = "", PhoneNumber = ""});
            Console.WriteLine("Received: " + response);
            List<GroupResponse> groups = new List<GroupResponse>();
            groups = JsonConvert.DeserializeObject<List<GroupResponse>>(response);
            _rpcClient.Close();
            return Ok(groups);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostGroup request) {
        try {
            Console.WriteLine("Sending a request to create a user...");
            var response = await _rpcClient.CallAsync(Operation.CreateGroup, new GroupDTO { Name = request.Name, UserId  = request.UserId});
            Console.WriteLine("Received: " + response);
            _rpcClient.Close();
            return Ok("Group created successfully");
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
    }
    
    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinGroupReq request) {
        if(request.Token == 1 && request.UserId == 1) {
            return Ok("User joined group");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 and 1 for both ids");
    }
}
