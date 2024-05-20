using Domain;

namespace ExpenseRepository.Repository;

public class ExpenseRepository : IExpenseRepository {

    private readonly ExpenseRepositoryContext _context;

    public ExpenseRepository(ExpenseRepositoryContext context) {
        _context = context;
        CreateDB();
    }

    public List<Expense> GetExpensesFromUser(int groupId, int userId) {
        try {
            return _context.UserExpenseTable
                .Where(ue => ue.UserId == userId)
                .Join(_context.ExpenseTable,
                    ue => ue.ExpenseId,
                    e => e.Id,
                    (ue, e) => e)
                .Where(e => e.GroupId == groupId)
                .Distinct()
                .ToList();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while getting the expenses.", e);
        }
    }
    
    public List<Expense> GetExpensesFromGroup(int groupId) {
        try {
            return _context.ExpenseTable
                .Where(e => e.GroupId == groupId)
                .ToList();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while getting the expenses.", e);
        }
    }

    public Expense Create(Expense expense) {
        try {
            _context.ExpenseTable.Add(expense);
            _context.SaveChanges();
            AddUserExpense(expense.OwnerId, expense.Id);
            return expense;
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the expense.", ex);
        }
    }

    private void AddUserExpense(int userId, int expenseId) {
        try {
            var userExpense = new UserExpense {
                UserId = userId,
                ExpenseId = expenseId
            };
            _context.UserExpenseTable.Add(userExpense);
            _context.SaveChanges();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while adding the user expense.", e);
        }
    }
    private void CreateDB() {
        try {
            Console.WriteLine("Creating database...");
            _context.Database.EnsureCreated();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while creating the database.", e);
        }

    }
}
