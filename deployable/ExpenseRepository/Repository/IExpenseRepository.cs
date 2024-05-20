using Domain;

namespace ExpenseRepository.Repository;

public interface IExpenseRepository {
    public List<Expense> GetExpensesFromUser(int groupId, int userId);
    public List<Expense> GetExpensesFromGroup(int groupId);
    public Expense Create(Expense expense);
}
