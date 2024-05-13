using AuthService.Services;
using Messages.Auth;
using Messages.User;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase {
    private readonly Services.AuthService _service;
    
    public AuthController(Services.AuthService service) {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserReq request) {
        try {
            var token = await _service.Register(request);
            return Ok(token);
        } catch (HttpRequestException) {
            // Thrown when cannot connect to user-service (send request for creating user)
            return StatusCode(502, "UserService is down");
        }
        catch (Exception e) {
            // Handled the same way as in UserService
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserReq request) {
        try {
            var token = await _service.Login(request);
            return Ok(token);
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}
