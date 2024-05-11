using Domain.DTO.Group;
using Domain.packages;
using Messages.Expense;
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

    [HttpPost("GetExpensesFromUserInGroup")]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromUserInGroup([FromBody] ExpenseDto request) {
        try {
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromUserInGroup, new ExpenseDto() { UserId = request.UserId, GroupId = request.GroupId });
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();
            expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from user in group");
        }
    }

    [HttpGet("{groupId}/expenses")]
    public async Task<ActionResult<List<ExpenseResponse>>> AllExpensesFromGroup([FromRoute] int groupId) {
        try {
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromGroup,
                new ExpenseDto() { GroupId = groupId, UserId = 0 });
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();
            expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from group");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] PostExpense request) {
        try {
            var response = await _rpcClient.CallAsync(Operation.CreateExpense, new PostExpense { Amount = request.Amount, Description = request.Description, GroupId = request.GroupId, OwnerId = request.OwnerId, Participants = request.Participants, Date = DateTime.Now });
            ExpenseResponse expense = JsonConvert.DeserializeObject<ExpenseResponse>(response);
            return Ok(expense);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating expense");
        }
    }
}
