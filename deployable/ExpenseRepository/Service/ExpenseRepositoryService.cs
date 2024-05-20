using Domain;
using ExpenseRepository.Repository;
using Messages.Expense;
using Messages.Expense.Dto;
using Messages.Expense.Request;
using Messages.Expense.Response;

namespace ExpenseRepository.Service;

public class ExpenseRepositoryService {
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseRepositoryService(IExpenseRepository expenseRepository) {
        _expenseRepository = expenseRepository;
    }

    public List<ExpenseResponse> GetExpensesFromUser(GetExpensesUserReq request) {
        return _expenseRepository.GetExpensesFromUser(request.GroupId, request.UserId)
            .Select(expense => new ExpenseResponse() {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                GroupId = expense.GroupId,
                Date = expense.Date,
                OwnerId = expense.OwnerId,
                Participants = expense.Participants
            })
            .ToList();
    }
    
    public List<ExpenseResponse> GetExpensesFromGroup(GetExpensesReq request) {
        return _expenseRepository.GetExpensesFromGroup(request.GroupId)
            .Select(expense => new ExpenseResponse() {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                GroupId = expense.GroupId,
                Date = expense.Date,
                OwnerId = expense.OwnerId,
                Participants = expense.Participants
            }).ToList();
    }

    public ExpenseResponse Create(PostExpense request) {
        var expense = new Expense() {
            Amount = request.Amount,
            Description = request.Description,
            GroupId = request.GroupId,
            Date = request.Date,
            OwnerId = request.OwnerId,
            Participants = request.Participants
        };
        
        var created = _expenseRepository.Create(expense);
        
        return new ExpenseResponse() {
            Id = created.Id,
            Amount = created.Amount,
            Description = created.Description,
            GroupId = created.GroupId,
            Date = created.Date,
            OwnerId = created.OwnerId,
            Participants = created.Participants
        };
    }
}
