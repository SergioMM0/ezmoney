using Domain;
using Domain.DTO.Expense;
using Domain.DTO.Group;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {
    
    private readonly RpcClient _rpcClient;
    
    public ExpenseController() {
        _rpcClient = new RpcClient("Expense_queue");
    }
    
    [HttpPost("GetExpensesFromUserInGroup")]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromUserInGroup([FromBody] ExpenseDTO request) {
        try {
            Console.WriteLine("Sending a request to get expenses from user in group...");
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromUserInGroup, new ExpenseDTO() { UserId = request.UserId, GroupId = request.GroupId });
            Console.WriteLine("Received: " + response);
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();
            expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            _rpcClient.Close();
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
    }
    
    [HttpGet("{groupId}/expenses")]
    public async Task<ActionResult<List<ExpenseResponse>>> AllExpensesFromGroup([FromRoute] int groupId) {
        try {
            Console.WriteLine("Sending a request to retrieve the expenses...");
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromGroup, new ExpenseDTO() { GroupId = groupId, UserId = 0});
            Console.WriteLine("Received: " + response);
            _rpcClient.Close();
            List<ExpenseResponse> expenses = new List<ExpenseResponse>();
            expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostExpense request) {
        try {
            Console.WriteLine("Sending a request to create a expense...");
            var response = await _rpcClient.CallAsync(Operation.CreateExpense, new PostExpense { Amount = request.Amount, Description = request.Description, GroupId = request.GroupId, OwnerId = request.OwnerId, Participants = request.Participants, Date = DateTime.Now});
            Console.WriteLine("Received: " + response);
            _rpcClient.Close();
            return Ok("Expense created successfully");
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("No bueno");
        }
    }
}
