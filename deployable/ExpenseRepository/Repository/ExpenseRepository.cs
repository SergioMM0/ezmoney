using Domain;
using Messages.Expense;

namespace ExpenseRepository.Repository;

public class ExpenseRepository : IExpenseRepository {

    private readonly ExpenseRepositoryContext _context;

    public ExpenseRepository(ExpenseRepositoryContext context) {
        _context = context;
        CreateDB();
    }

    public List<Expense> GetExpenseFromUserInGroup(ExpenseDto expenseDto) {
        try {
            return _context.UserExpenseTable
                .Where(ue => ue.UserId == expenseDto.UserId)
                .Join(_context.ExpenseTable,
                    ue => ue.ExpenseId,
                    e => e.Id,
                    (ue, e) => e)
                .Where(e => e.GroupId == expenseDto.GroupId)
                .Distinct()
                .ToList();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while getting the expenses.", e);
        }

    }

    public Expense AddExpense(PostExpense expense) {
        Expense newExpense = new Expense() {
            Amount = expense.Amount,
            Description = expense.Description,
            GroupId = expense.GroupId,
            Date = expense.Date,
            OwnerId = expense.OwnerId,
            Participants = expense.Participants
        };

        try {
            _context.ExpenseTable.Add(newExpense);
            _context.SaveChanges();
            AddUserExpense(expense.OwnerId, newExpense.Id);
            return newExpense;
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the expense.", ex);
        }
    }

    public List<Expense> GetExpensesFromGroup(ExpenseDto expenseDto) {
        try {
            return _context.ExpenseTable
                .Where(e => e.GroupId == expenseDto.GroupId)
                .ToList();
        } catch (Exception e) {
            throw new ApplicationException("An error occurred while getting the expenses.", e);
        }

    }

    public void AddUserExpense(int userId, int expenseId) {
        try {
            UserExpense userExpense = new UserExpense {
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
