using Domain.DTO.Expense;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {
    
    [HttpGet("{groupId}/expenses")]
    public IActionResult AllExpensesFromGroup([FromRoute] int groupId) {
        if(groupId == 1) {
            return Ok("many expenses ayayay");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] PostExpense request) {
        if(request.GroupId == 1) {
            return Ok("da good group 1 has a new expense");
        }
        return BadRequest("No bueno, to test this endpoint OK result insert 1 for group id");
    }
}
