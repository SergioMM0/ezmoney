using Domain;
using Messages.Group;
using Messages.Group.Dto;
using Messages.Group.Request;
using Messages.Group.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPC;

namespace GroupService.Controller;

[ApiController]
[Route("group")]
public class GroupController(RpcClient rpcClient) : ControllerBase {

    /// <summary>
    /// Retrieves all groups from a user based on userId.
    /// <br/> - Sends a request through RPC to:
    /// <br/> - GroupRepository
    /// <br/> - UserRepository
    /// <br/>
    /// TODO: Add check for user exists
    /// </summary>
    /// <param name="userId"><see cref="int"/></param>
    /// <returns>A <see cref="IActionResult"/> containing a <see cref="List{T}"/> of <see cref="GroupResponse"/> objects.</returns>
    [HttpGet("{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<GroupResponse>>> GetAllGroupsOfUser([FromRoute] int userId) {
        try {
            // Create request object to send to the group repository
            var groupsUserReq = new GroupsUserReq() {
                UserId = userId
            };
            
            // Fetches all groups from user
            var response = await rpcClient.CallAsync(Operation.GetGroupsFromUser, groupsUserReq);
            var groups = JsonConvert.DeserializeObject<List<GroupResponse>>(response);
            return Ok(groups);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting groups from user");
        }
    }

    /// <summary>
    /// Process the request to create a new group.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- GroupRepository
    /// <br/>- UserRepository
    /// <br/>
    /// <br/>-TODO: Add check for user exists
    /// </summary>
    /// <param name="request"><see cref="PostGroup"/></param>
    /// <returns>A <see cref="IActionResult"/> of <see cref="GroupResponse"/></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupResponse>> Create([FromBody] PostGroup request) {
        try {
            // Create request object to send to the group repository
            var createGroupReq = new CreateGroupReq() {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Token = GenerateGroupToken()
            };
            
            // Sends the request to the group repository
            var response = await rpcClient.CallAsync(Operation.CreateGroup, createGroupReq);
            
            // Deserializes the response and returns it
            var group = JsonConvert.DeserializeObject<GroupResponse>(response);
            return Ok(group);
            
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating group");
        }
    }

    /// <summary>
    /// Process the request to join a group.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- GroupRepository
    /// <br/>- UserRepository
    /// </summary>
    /// <param name="request"><see cref="JoinGroupReq"/></param>
    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Join([FromBody] JoinGroupReq request) {
        try {
            // Create request object to send to the group repository
            var joinGroupReq = new JoinGroupReq() {
                UserId = request.UserId,
                Token = request.Token
            };
            
            // Sends the request to the group repository
            var response = rpcClient.CallAsync(Operation.JoinGroup, joinGroupReq);
            
            
            return NoContent();
            
        } catch (Exception e) {
            Console.WriteLine(e);
            return StatusCodes.Status500InternalServerError("Error joining group");
        }
    }
    
    private string GenerateGroupToken() {
        return Guid.NewGuid().ToString();
    }
}
