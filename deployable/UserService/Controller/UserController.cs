using System.Diagnostics;
using Messages.User.Dto;
using Messages.User.Request;
using Messages.User.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using RPC;

namespace UserService.Controller;

[ApiController]
[Route("user")]
public class UserController : ControllerBase {
    private readonly RpcClient _rpcClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policies;

    public UserController(RpcClient rpcClient, IAsyncPolicy<HttpResponseMessage> policies) {
        _rpcClient = rpcClient;
        _policies = policies;
    }
    
    [HttpGet("{phoneNumber}")]
    public async Task<ActionResult<UserResponse>> GetByPhoneNumber([FromRoute] string phoneNumber)
    {
        try
        {
            var request = new GetUserByPhone()
            {
                PhoneNumber = phoneNumber
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetUserByPhoneNumber, request);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent.Contains("null"))
                return NotFound("User not found");

            var user = JsonConvert.DeserializeObject<UserResponse>(responseContent);
            return Ok(user);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetUserByPhoneNumber::Circuit breaker is open, request aborted.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service is temporarily unavailable. Please try again later.");
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting user by phone number");
            Console.WriteLine(e);
            return BadRequest("Error logging in: " + e.Message);
        }
    }

    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers() {
        try {
            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetAllUsers, null);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserResponse>>(responseContent);
            return Ok(users);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetAllUsers::Circuit breaker is open, request aborted.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service is temporarily unavailable. Please try again later.");
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting users");
            Console.WriteLine(e);
            return BadRequest("Error getting users: " + e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostUser dto) {
        try {
            var request = new CreateUserReq() {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.CreateUser, request);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserResponse>(responseContent);
            return Ok(user);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("Create::Circuit breaker is open, request aborted.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service is temporarily unavailable. Please try again later.");
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error creating user");
            Console.WriteLine(e);
            return BadRequest("Error creating user: " + e.Message);
        }
    }
}
