using Messages.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase {
    private readonly Services.AuthService _service;
    
    public AuthController(Services.AuthService service) {
        _service = service;
    }
    
    /// <summary>
    /// Register the user in the system.
    /// </summary>
    /// <param name="userId"><see cref="RegisterUserReq"/></param>
    /// <returns>A <see cref="IActionResult"/> containing the Bearer Token as <see cref="string"/></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof (string))]
    public async Task<IActionResult> Register([FromBody] RegisterUserReq request) {
        try {
            var token = await _service.Register(request);
            return Ok(token);
        } catch (HttpRequestException) {
            // Thrown when cannot connect to user-service (send request for creating user)
            return StatusCode(503, "UserService is down");
        }
        catch (Exception e) {
            // Handled the same way as in UserService
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// Logs the user in the system.
    /// </summary>
    /// <param name="request"><see cref="LoginUserReq"/></param>
    /// <returns>A <see cref="IActionResult"/> containing the Bearer Token as <see cref="string"/></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginUserReq request) {
        try {
            var token = await _service.Login(request);
            return Ok(token);
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}
