using ExpenseRepository.Service;
using Messages.Expense.Dto;
using Messages.Expense.Request;
using Messages.Expense.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseRepository.Controllers; 

[ApiController]
[Route("[controller]")]
public class ExpenseRepositoryController : ControllerBase{
    private readonly ExpenseRepositoryService _expenseRepositoryService;
    public ExpenseRepositoryController(ExpenseRepositoryService expenseRepositoryService){
        _expenseRepositoryService = expenseRepositoryService;
    }
    
    [HttpGet("GetExpensesFromUser")]
    public ActionResult<List<ExpenseResponse>> GetExpensesFromUser(GetExpensesUserReq request){
        try{
            return Ok(_expenseRepositoryService.GetExpensesFromUser(request));
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    [HttpGet("GetExpensesFromGroup")]
    public ActionResult<List<ExpenseResponse>> GetExpensesFromGroup(GetExpensesReq request){
        try{
            return Ok(_expenseRepositoryService.GetExpensesFromGroup(request));
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    [HttpPost("CreateExpense")]
    public ActionResult<ExpenseResponse> CreateExpense(PostExpense request){
        try{
            return Ok(_expenseRepositoryService.Create(request));
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    
}
