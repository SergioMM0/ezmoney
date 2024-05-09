using ExpenseRepository.Repository;

namespace ExpenseRepository.Service; 

public class ExpenseRepositoryService {
    private readonly IExpenseRepository _expenseRepository;
    
    public ExpenseRepositoryService(IExpenseRepository expenseRepository) {
        _expenseRepository = expenseRepository;
    }
    
}
