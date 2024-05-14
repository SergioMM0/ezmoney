using FluentValidation;
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
public class GroupController : ControllerBase {
    private readonly RpcClient _rpcClient;
    
    public GroupController(RpcClient rpcClient) {
        _rpcClient = rpcClient;
    }

    /// <summary>
    /// Retrieves all groups from a user based on userId.
    /// <br/> - Sends a request through RPC to:
    /// <br/> - GroupRepository
    /// <br/> - UserRepository
    /// <br/>
    /// TODO: Add check for user exists
    /// </summary>
    /// <param name="userId"><see cref="int"/></param>
    /// <returns>A <see cref="IActionResult"/> containing an <see cref="List{T}"/> of <see cref="GroupResponse"/> objects.</returns>
    [HttpGet("{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<List<GroupResponse>>> GetAllGroupsOfUser([FromRoute] int userId) {
        try {
            if (userId <= 0) {
                return BadRequest("Invalid user id");
            }
            
            // Create request object to send to the group repository
            var groupsUserReq = new GroupsUserReq() {
                UserId = userId
            };
            
            // Fetches all groups from user
            var groupResponse = await _rpcClient.CallAsync(Operation.GetGroupsFromUser, groupsUserReq);
            var groups = JsonConvert.DeserializeObject<List<GroupResponse>>(groupResponse);
            return Ok(groups);
        } catch (Exception e) {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }
    
    /*
    [HttpGet("{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<List<GroupResponse>>> GetById([FromRoute] int groupId) {
        try {
            if (groupId <= 0) {
                return BadRequest("Invalid user id");
            }
            
            // Create request object to send to the group repository
            var groupRequest = new GroupByIdRequest() {
                UserId = groupId
            };
            
            // Fetches all groups from user
            var groupResponse = await _rpcClient.CallAsync(Operation.GetGroupsFromUser, groupRequest);
            var groups = JsonConvert.DeserializeObject<List<GroupResponse>>(groupResponse);
            return Ok(groups);
        } catch (Exception e) {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }
    
    */

    /// <summary>
    /// Process the request to create a new group.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- GroupRepository
    /// <br/>- UserRepository
    /// <br/>
    /// <br/>-TODO: Add check for user exists
    /// </summary>
    /// <param name="request"><see cref="PostGroup"/></param>
    /// <returns>An <see cref="IActionResult"/> of <see cref="GroupResponse"/></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<GroupResponse>> Create([FromBody] PostGroup request) {
        try {
            
            // Create request object to send to the group repository
            var createGroupReq = new CreateGroupReq() {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Token = GenerateGroupToken()
            };
            
            // Sends the request to the group repository
            var response = await _rpcClient.CallAsync(Operation.CreateGroup, createGroupReq);
            
            // Deserializes the response and returns it
            var group = JsonConvert.DeserializeObject<GroupResponse>(response);
            return Ok(group);
            
        } catch (Exception e) {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }

    /// <summary>
    /// Process the request to join a group.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- GroupRepository
    /// <br/>- UserRepository
    /// TODO: Add check for user exists
    /// </summary>
    /// <param name="request"><see cref="JoinGroupReq"/></param>
    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<IActionResult> Join([FromBody] JoinGroupReq request) {
        try {
            // Create request object to send to the group repository
            var joinGroupReq = new JoinGroupReq() {
                UserId = request.UserId,
                Token = request.Token
            };
            
            // Sends the request to the group repository
            var response = await _rpcClient.CallAsync(Operation.JoinGroup, joinGroupReq);
            var contentString = JsonConvert.DeserializeObject<string>(response);
            
            return Ok(contentString);
        } catch (Exception e) {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }
    
    private string GenerateGroupToken() {
        return Guid.NewGuid().ToString();
    }
}
