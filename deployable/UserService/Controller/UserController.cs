using Domain;
using Messages.User;
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
    [HttpGet("phone/{phoneNumber}")]
    public async Task<ActionResult<UserResponse>> GetByPhoneNumber([FromRoute] string phoneNumber) {
        Console.WriteLine("Getting user by phone number: " + phoneNumber);
        try {
            var response = await _rpcClient.CallAsync(Operation.GetUserByPhoneNumber,
                new User { Name = "", PhoneNumber = phoneNumber });
            if (response.Contains("null"))
                return NotFound("User not found");
            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(response);
            return Ok(user);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error logging in: " + e.Message);
        }
    }

    /// <summary>
    /// Attempts to create a user in the system, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserReq request) {
        try {
            var response = await _rpcClient.CallAsync(Operation.CreateUser, new User { Name = request.Name, PhoneNumber = request.PhoneNumber });
            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(response);
            return Ok(user);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating user");
        }

    }

    [HttpPost("GetAllUsers")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers() {
        try {
            var response = await _rpcClient.CallAsync(Operation.GetAllUsers, null);
            List<UserResponse> users = new List<UserResponse>();
            users = JsonConvert.DeserializeObject<List<UserResponse>>(response);
            return Ok(users);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting users");
        }

    }
}
