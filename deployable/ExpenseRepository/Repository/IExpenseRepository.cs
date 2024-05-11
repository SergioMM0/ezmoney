using Domain;
using Domain.DTO.Group;
using Messages.Expense;

namespace ExpenseRepository.Repository;

public interface IExpenseRepository {
    public List<Expense> GetExpenseFromUserInGroup(ExpenseDto expenseDto);
    public Expense AddExpense(PostExpense expense);
    public List<Expense> GetExpensesFromGroup(ExpenseDto expenseDto);
}
