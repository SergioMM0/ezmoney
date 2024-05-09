using Domain;
using Domain.DTO.User;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;

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
        if (!request.PhoneNumber.Equals("")) {
            var response = await _rpcClient.CallAsync(Operation.LoginUser, new User {Name = "", PhoneNumber = request.PhoneNumber });
            Console.WriteLine("Received: " + response);
            _rpcClient.Close();
            return Ok("User Logged successfully");
        }
        return BadRequest("No bueno");
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
            Console.WriteLine("Sending a request to create a user...");
            var response = await _rpcClient.CallAsync(Operation.CreateUser, new User { Name = request.Name, PhoneNumber = request.PhoneNumber  });
            Console.WriteLine("Received: " + response);
            _rpcClient.Close();
            return Ok("User created successfully");
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
        
    }
    
    [HttpPost("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers() {
        
        Console.WriteLine("Sending a request to get all users...");
        var response = await _rpcClient.CallAsync(Operation.GetAllUsers, null);
        Console.WriteLine("Received: " + response);
        _rpcClient.Close();
        return Ok(response);
    }
}
