using Domain;
using Domain.DTO.Expense;
using Domain.DTO.Group;
using ExpenseRepository.Repository;

namespace ExpenseRepository.Service;

public class ExpenseRepositoryService {
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseRepositoryService(IExpenseRepository expenseRepository) {
        _expenseRepository = expenseRepository;
    }

    public List<Expense> GetExpenseFromUserInGroup(ExpenseDTO expenseDto) {
        return _expenseRepository.GetExpenseFromUserInGroup(expenseDto);
    }

    public Expense AddExpense(PostExpense expense) {
        return _expenseRepository.AddExpense(expense);
    }

    public List<Expense> GetExpensesFromGroup(ExpenseDTO expenseDto) {
        return _expenseRepository.GetExpensesFromGroup(expenseDto);
    }
}
