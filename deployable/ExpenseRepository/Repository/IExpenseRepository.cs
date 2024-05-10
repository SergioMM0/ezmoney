using Domain;
using Domain.DTO.Expense;
using Domain.DTO.Group;

namespace ExpenseRepository.Repository;

public interface IExpenseRepository {
    public List<Expense> GetExpenseFromUserInGroup(ExpenseDTO expenseDto);
    public Expense AddExpense(PostExpense expense);
    public List<Expense> GetExpensesFromGroup(ExpenseDTO expenseDto);
}
