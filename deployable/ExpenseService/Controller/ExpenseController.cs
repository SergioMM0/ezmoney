using Domain.DTO.Expense;
using Domain.packages;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {
    
    private readonly RpcClient _rpcClient;
    
    public ExpenseController(RpcClient rpcClient) {
        _rpcClient = new RpcClient("expense_queue");
    }
    
    
    
    
    [HttpGet("{groupId}/expenses")]
    public IActionResult AllExpensesFromGroup([FromRoute] int groupId) {
        if(groupId == 1) {
            return Ok("many expenses ayayay");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
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
