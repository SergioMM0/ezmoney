using Messages.Expense.Dto;
using Messages.Expense.Request;
using Messages.Expense.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using Polly;
using Polly.CircuitBreaker;
using RPC;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {
    private readonly RpcClient _rpcClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policies;
    private readonly IHttpClientFactory _clientFactory;

    public ExpenseController(RpcClient rpcClient, IAsyncPolicy<HttpResponseMessage> policies, IHttpClientFactory clientFactory) {
        _rpcClient = rpcClient;
        _policies = policies;
        _clientFactory = clientFactory;
    }

    [HttpGet("{groupId}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExpenseResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromUser([FromRoute] int groupId, [FromRoute] int userId) {
        try {
            var request = new GetExpensesUserReq() {
                UserId = userId,
                GroupId = groupId
            };
            
            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetExpensesFromUserInGroup, request);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(responseContent);
            return Ok(expenses);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetExpensesFromUser::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("ExpenseRepoHTTP");
            var response = await client.GetAsync($"http://expense-repo:8080/ExpenseRepository/GetExpensesFromUser/{groupId}/User/{userId}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<ExpenseResponse>>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            Monitoring.Monitoring.Log.Error("GetExpensesFromUser::Request timed out");
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting expenses from user in group");
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from user in group");
        }
    }

    [HttpGet("{groupId}/expenses")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExpenseResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromGroup([FromRoute] int groupId) {
        try {
            var request = new GetExpensesReq() {
                GroupId = groupId
            };
            
            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.GetExpensesFromGroup, request);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(responseContent);
            return Ok(expenses);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("GetExpensesFromGroup::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("ExpenseRepoHTTP");
            var response = await client.GetAsync($"http://expense-repo:8080/ExpenseRepository/GetExpensesFromGroup/{groupId}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<ExpenseResponse>>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            Monitoring.Monitoring.Log.Error("GetExpensesFromGroup::Request timed out");
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error getting expenses from group");
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from group");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExpenseResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] PostExpense request) {
        try {
            var expenseRequest = new CreateExpenseReq {
                Amount = request.Amount,
                Description = request.Description,
                GroupId = request.GroupId,
                OwnerId = request.OwnerId,
                Participants = request.Participants,
                Date = request.Date
            };

            var response = await _policies.ExecuteAsync(async () =>
            {
                var rpcResponse = await _rpcClient.CallAsync(Operation.CreateExpense, expenseRequest);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(rpcResponse)
                };
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            var expense = JsonConvert.DeserializeObject<ExpenseResponse>(responseContent);
            return Ok(expense);
        }
        catch (BrokenCircuitException)
        {
            Monitoring.Monitoring.Log.Error("Create::Circuit breaker is open, fallback strategy launched.");
            var client = _clientFactory.CreateClient("ExpenseRepoHTTP");
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://expense-repo:8080/ExpenseRepository/CreateExpense", content);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ExpenseResponse>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Ok(result);
        }
        catch (RpcTimeoutException)
        {
            Monitoring.Monitoring.Log.Error("Create::Request timed out");
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request timed out");
        }
        catch (Exception e)
        {
            Monitoring.Monitoring.Log.Error("Error creating expense");
            Console.WriteLine(e);
            return BadRequest("Error creating expense");
        }
    }
}
