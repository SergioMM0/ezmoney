using Domain;
using Domain.DTO.Expense;
using Domain.DTO.Group;

namespace ExpenseRepository.Repository; 

public class ExpenseRepository : IExpenseRepository{
    
    private readonly ExpenseRepositoryContext _context;
    
    public ExpenseRepository(ExpenseRepositoryContext context) {
        _context = context;
        CreateDB();
    }

    public List<Expense> GetExpenseFromUserInGroup(ExpenseDTO expenseDto) {
        return _context.UserExpenseTable
            .Where(ue => ue.UserId == expenseDto.UserId) 
            .Join(_context.ExpenseTable,            
                ue => ue.ExpenseId,                   
                e => e.Id,                           
                (ue, e) => e)                          
            .Where(e => e.GroupId == expenseDto.GroupId) 
            .Distinct()                                 
            .ToList();     
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
            throw new ApplicationException("An error occurred while adding the group.", ex);
        }
    }

    public List<Expense> GetExpensesFromGroup(ExpenseDTO expenseDto) {
        return _context.ExpenseTable
            .Where(e => e.GroupId == expenseDto.GroupId)
            .ToList();
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
            Console.WriteLine(e);
            throw;
        }
    }
    private void CreateDB() {
        Console.WriteLine("Creating database...");
        _context.Database.EnsureCreated();
    }
}
