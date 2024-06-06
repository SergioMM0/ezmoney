using Messages.Group;
using Messages.Group.Dto;
using Messages.Group.Request;
using Messages.Group.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using RPC;

namespace GroupService.Controller;

[ApiController]
[Route("group")]
public class GroupController : ControllerBase {
    private readonly RpcClient _rpcClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policies;
    private readonly IHttpClientFactory _clientFactory;

    public GroupController(RpcClient rpcClient, IAsyncPolicy<HttpResponseMessage> policies, IHttpClientFactory httpClientFactory) {
        _rpcClient = rpcClient;
        _policies = policies;
        _clientFactory = httpClientFactory;
    }

    [HttpGet("{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<List<GroupResponse>>> GetAllGroupsOfUser([FromRoute] int userId) {
        try {
            if (userId <= 0) {
                return BadRequest("Invalid user id");
            }

            var groupsUserReq = new GroupsUserReq() {
                UserId = userId
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetGroupsFromUser, groupsUserReq);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var groups = JsonConvert.DeserializeObject<List<GroupResponse>>(responseContent);
            return Ok(groups);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetAllGroupsOfUser::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("GroupRepoHTTP");
            var response = await client.GetAsync($"http://group-repo:8080/{userId}/groups");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<GroupResponse>>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting groups from user");
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }

    [HttpGet("{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<GroupResponse>> GetById([FromRoute] int groupId) {
        try {
            if (groupId <= 0) {
                return BadRequest("Invalid group id");
            }

            var groupRequest = new GroupByIdRequest() {
                GroupId = groupId
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetGroupById, groupRequest);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var group = JsonConvert.DeserializeObject<GroupResponse>(responseContent);
            return Ok(group);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetById::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("GroupRepoHTTP");
            var response = await client.GetAsync($"http://group-repo:8080/group/{groupId}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<GroupResponse>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting group by id");
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<GroupResponse>> Create([FromBody] PostGroup request) {
        try {
            var createGroupReq = new CreateGroupReq() {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Token = GenerateGroupToken()
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.CreateGroup, createGroupReq);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var group = JsonConvert.DeserializeObject<GroupResponse>(responseContent);
            return Ok(group);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("Create::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("GroupRepoHTTP");
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://group-repo:8080/group", content);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<GroupResponse>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error creating group");
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }

    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<IActionResult> Join([FromBody] JoinGroupReq request) {
        try {
            var joinGroupReq = new JoinGroupReq() {
                UserId = request.UserId,
                Token = request.Token
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.JoinGroup, joinGroupReq);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            return Ok(responseContent);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("Join::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("GroupRepoHTTP");
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://group-repo:8080/group/join", content);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Ok(jsonResponse);
        }
        catch (RpcTimeoutException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error joining group");
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Couldn't deserialize the response");
        }
    }

    private string GenerateGroupToken() {
        return Guid.NewGuid().ToString();
    }
}
