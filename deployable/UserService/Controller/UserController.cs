using Domain;
using Domain.DTO.User;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace UserService.Controller;

[ApiController]
[Route("user")]
public class UserController : ControllerBase {
    private readonly RpcClient _rpcClient;
    public UserController() {
        _rpcClient = new RpcClient("user_queue");
    }
    /// <summary>
    /// Attempts to log in a user, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository to check if the user exists through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns>{IActionResult}</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserReq request) {
        if (!string.IsNullOrEmpty(request.PhoneNumber)) {
            try {
                var response = await _rpcClient.CallAsync(Operation.LoginUser, new User { Name = "", PhoneNumber = request.PhoneNumber });
                Console.WriteLine("Received: " + response);
                _rpcClient.Close();
                if (response.Contains("null"))
                    return BadRequest("User not found");
                return Ok("User Logged successfully");
            } catch (Exception e) {
                Console.WriteLine(e);
                return BadRequest("Error logging in: " + e.Message);
            }
        }
        return BadRequest("Invalid phone number");
    }

    /// <summary>
    /// Attempts to register a user in the system, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserReq request) {
        try {
            var response = await _rpcClient.CallAsync(Operation.CreateUser, new User { Name = request.Name, PhoneNumber = request.PhoneNumber });
            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(response);
            _rpcClient.Close();
            return Ok(user);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error registering user");
        }

    }

    [HttpPost("GetAllUsers")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers() {
        try {
            var response = await _rpcClient.CallAsync(Operation.GetAllUsers, null);
            List<UserResponse> users = new List<UserResponse>();
            users = JsonConvert.DeserializeObject<List<UserResponse>>(response);
            _rpcClient.Close();
            return Ok(users);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting users");
        }

    }
}
