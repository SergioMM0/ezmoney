using Messages.User.Dto;
using Messages.User.Request;
using Messages.User.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPC;

namespace UserService.Controller;

[ApiController]
[Route("user")]
public class UserController : ControllerBase {
    private readonly RpcClient _rpcClient;
    
    public UserController(RpcClient rpcClient) {
        _rpcClient = rpcClient;
    }
    
    /// <summary>
    /// Attempts to get a user by phone number, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository to check if the user exists through RabbitMQ
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns>{IActionResult}</returns>
    [HttpGet("{phoneNumber}")]
    public async Task<ActionResult<UserResponse>> GetByPhoneNumber([FromRoute] string phoneNumber) {
        try {
            var request = new GetUserByPhone() {
                PhoneNumber = phoneNumber
            };
            var response = await _rpcClient.CallAsync(Operation.GetUserByPhoneNumber, request);
            if (response.Contains("null"))
                return NotFound("User not found");
            var user = JsonConvert.DeserializeObject<UserResponse>(response);
            return Ok(user);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error logging in: " + e.Message);
        }
    }
    
    /// <summary>
    /// Retrieves all the users in the system
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers() {
        try {
            var response = await _rpcClient.CallAsync(Operation.GetAllUsers, null);
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(response);
            return Ok(users);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting users");
        }
    }

    /// <summary>
    /// Attempts to create a user in the system, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository through RabbitMQ
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostUser dto) {
        try {
            var request = new CreateUserReq() {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber
            };

            var response = await _rpcClient.CallAsync(Operation.CreateUser, request);
            var user = JsonConvert.DeserializeObject<UserResponse>(response);
            return Ok(user);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating user");
        }
    }
}
