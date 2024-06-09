using System.Diagnostics;
using Messages.User.Dto;
using Messages.User.Request;
using Messages.User.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
    private readonly IHttpClientFactory _clientFactory;
    public UserController(RpcClient rpcClient, IAsyncPolicy<HttpResponseMessage> policies,IHttpClientFactory httpClientFactory) {
        _rpcClient = rpcClient;
        _policies = policies;
        _clientFactory = httpClientFactory;
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
            Monitoring.Monitoring.Log.Error("GetUserByPhoneNumber::Circuit breaker is open, fallback startegy launched.");
            var client = _clientFactory.CreateClient("UserRepoHTTP");
            var response = await client.GetAsync($"http://user-repo:8080/user/{phoneNumber}");
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<double>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
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
            Monitoring.Monitoring.Log.Error("GetAllUsers::Circuit breaker is open, request aborted, implementing fallback strategy.");
            var client = _clientFactory.CreateClient("UserRepoHTTP");
            var response = await client.GetAsync($"http://user-repo:8080/GetAllUsers");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<double>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
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

            Monitoring.Monitoring.Log.Information("Create::Creating user...");
            var response = await _policies.ExecuteAsync(async () =>
            {
                Monitoring.Monitoring.Log.Verbose("Create::Calling RPC...");
                var rpcResponse = await _rpcClient.CallAsync(Operation.CreateUser, request);
                Monitoring.Monitoring.Log.Verbose("Create::RPC response received.");
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
            Monitoring.Monitoring.Log.Error("Create::Circuit breaker is open, request aborted, implementing fallback strategy.");
            var client = _clientFactory.CreateClient("UserRepoHTTP");
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(dto);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://user-repo:8080/user", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<double>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "RPC response : Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error creating user");
            Console.WriteLine(e);
            return BadRequest("Error creating user: " + e.Message);
        }
    }
}
