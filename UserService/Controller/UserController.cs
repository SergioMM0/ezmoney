using Domain.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controller;

[ApiController]
[Route("user")]
public class UserController : ControllerBase {
    /// <summary>
    /// Attempts to log in a user, returning a 200 OK if successful, 400 Bad Request if not
    /// Sends a petition to the UserRepository to check if the user exists through RabbitMQ
    /// </summary>
    /// <param name="request"></param>
    /// <returns>{IActionResult}</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginUserReq request) {
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
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterUserReq request) {
        if(request.Name == "admin") {
            return Ok("Login successful");
        }
        return BadRequest("No bueno");
    }
}
