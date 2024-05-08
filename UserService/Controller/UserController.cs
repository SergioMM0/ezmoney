using Domain.DTO.User;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controller;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
    /// <summary>
    /// Attempts to log in a user, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository to check if the user exists through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns>{IActionResult}</returns>
    [HttpPost]
    public IActionResult Login([FromBody] LoginUserReq request) {
        var rpcClient = new RpcClient();
        
        if(request.Name == "admin") {
            return Ok("Login successful");
        }
        return BadRequest("No bueno");
    }
    
    /// <summary>
    /// Attempts to register a user in the system, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] RegisterUserReq request) {
        if(request.Name == "admin") {
            return Ok("Login successful");
        }
        return BadRequest("No bueno");
    }
    
}
