using Messages.Expense.Dto;
using Messages.Expense.Request;
using Messages.Expense.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPC;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {

    private readonly RpcClient _rpcClient;

    public ExpenseController(RpcClient rpcClient) {
        _rpcClient = rpcClient;
    }

    [HttpGet("{groupId}/user/{userId}")]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromUser([FromRoute] int groupId, [FromRoute] int userId) {
        try {
            var request = new GetExpensesUserReq() {
                UserId = userId,
                GroupId = groupId
            };
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromUserInGroup, request);
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from user in group");
        }
    }

    [HttpGet("{groupId}/expenses")]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromGroup([FromRoute] int groupId) {
        try {
            var request = new GetExpensesReq() {
                GroupId = groupId
            };
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromGroup, request);
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from group");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] PostExpense request) {
        try {
            // Add datetime to the request
            var expenseRequest = new CreateExpenseReq {
                Amount = request.Amount,
                Description = request.Description,
                GroupId = request.GroupId,
                OwnerId = request.OwnerId,
                Participants = request.Participants,
                Date = request.Date
            };

            var response = await _rpcClient.CallAsync(Operation.CreateExpense, expenseRequest);
            var expense = JsonConvert.DeserializeObject<ExpenseResponse>(response);
            return Ok(expense);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating expense");
        }
    }
}
