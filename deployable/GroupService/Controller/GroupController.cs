using Domain;
using Messages.Group;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPC;

namespace GroupService.Controller;

[ApiController]
[Route("group")]
public class GroupController(RpcClient rpcClient) : ControllerBase {

    [HttpGet("{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<GroupResponse>>> GetAllGroupsOfUser([FromRoute] int userId) {
        try {
            var response = await rpcClient.CallAsync(Operation.GetGroupFromUser, new User { Id = userId, Name = "", PhoneNumber = "" });
            var groups = JsonConvert.DeserializeObject<List<GroupResponse>>(response);
            return Ok(groups);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting groups from user");
        }
    }

    /// <summary>
    /// Process the request to create a new group. Sends the request to the group repository.
    /// </summary>
    /// <param name="request"><c>PostGroup</c></param>
    /// <returns>GroupResponse object</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupResponse>> Create([FromBody] PostGroup request) {
        try {
            // Create object to send to the group repository
            var dto = new CreateGroupReq() {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Token = GenerateGroupToken()
            };
            
            // Sends the request to the group repository
            var response =
                await rpcClient.CallAsync(Operation.CreateGroup, dto);
            
            // Deserializes the response and returns it
            var group = JsonConvert.DeserializeObject<GroupResponse>(response);
            return Ok(group);
            
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating group");
        }
    }

    [HttpPost("join")]
    public IActionResult Join([FromBody] JoinGroupReq request) {
        if (request.Token == 1 && request.UserId == 1) {
            return Ok("User joined group");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 and 1 for both ids");
    }
    
    private string GenerateGroupToken() {
        return Guid.NewGuid().ToString();
    }

    /* NOT IN THE INITIAL REQUIREMENTS, MIGHT BE USED LATER
[HttpGet("/{groupId}/members")]
public IActionResult AllMembersFromGroup([FromRoute] int groupId) {
    if(groupId == 1) {
        return Ok("yo mama says hi");
    }
    return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
}
*/
}
