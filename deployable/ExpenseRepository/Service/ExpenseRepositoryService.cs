using Domain;
using Domain.DTO.Group;
using ExpenseRepository.Repository;
using Messages.Expense;

namespace ExpenseRepository.Service;

public class ExpenseRepositoryService {
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseRepositoryService(IExpenseRepository expenseRepository) {
        _expenseRepository = expenseRepository;
    }

    public List<Expense> GetExpenseFromUserInGroup(ExpenseDto expenseDto) {
        return _expenseRepository.GetExpenseFromUserInGroup(expenseDto);
    }

    public Expense AddExpense(PostExpense expense) {
        return _expenseRepository.AddExpense(expense);
    }

    public List<Expense> GetExpensesFromGroup(ExpenseDto expenseDto) {
        return _expenseRepository.GetExpensesFromGroup(expenseDto);
    }
}
