using Domain;
using ExpenseRepository.Repository;
using Messages.Expense.Dto;
using Messages.Expense.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseRepository.Controllers; 

[ApiController]
[Route("expense")]
public class ExpenseRepositoryController : ControllerBase{
    private readonly IExpenseRepository _repository;
    public ExpenseRepositoryController(IExpenseRepository repository){
        _repository = repository;
    }
    
    [HttpGet("{groupid}/user/{userId}")]
    public ActionResult<List<ExpenseResponse>> GetExpensesFromUser([FromRoute]int groupid, [FromRoute] int userId){
        try {
            var result = _repository.GetExpensesFromUser(groupid, userId)
                .Select(expense => new ExpenseResponse() {
                    Id = expense.Id,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    GroupId = expense.GroupId,
                    Date = expense.Date,
                    OwnerId = expense.OwnerId,
                    Participants = expense.Participants
                }).ToList();
            
            return Ok(result);
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public ActionResult<ExpenseResponse> CreateExpense([FromBody] PostExpense dto){
        try{
            var expense = new Expense() {
                Amount = dto.Amount,
                Description = dto.Description,
                GroupId = dto.GroupId,
                Date = dto.Date,
                OwnerId = dto.OwnerId,
                Participants = dto.Participants
            };
            var result = _repository.Create(expense);
            
            var mapped = new ExpenseResponse() {
                Id = result.Id,
                Amount = result.Amount,
                Description = result.Description,
                GroupId = result.GroupId,
                Date = result.Date,
                OwnerId = result.OwnerId,
                Participants = result.Participants
            };
            return Ok(mapped);
        } catch (Exception e){
            return BadRequest(e.Message);
        }
    }
    
}
