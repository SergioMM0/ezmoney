using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase {
    //Health check
    [HttpGet]
    public IActionResult Ping() {
        return Ok("Pong"); 
    }
}
