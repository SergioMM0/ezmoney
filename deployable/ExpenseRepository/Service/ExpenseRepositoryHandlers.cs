using Domain;
using Domain.DTO.Expense;
using Domain.packages;
using Domain.packages.Interfaces;
using Newtonsoft.Json;

namespace ExpenseRepository.Service; 

public class ExpenseRepositoryHandlers : IRequestHandler{
    
    private HandlerRegistry registry;

    private readonly ExpenseRepositoryService _expenseRepositoryService;
    
    public ExpenseRepositoryHandlers(ExpenseRepositoryService expenseRepositoryService=null) {
        registry = new HandlerRegistry();
        registry.RegisterHandler(Operation.CreateExpense, HandleCreateExpense);
        registry.RegisterHandler(Operation.GetExpensesFromGroup, HandleGetExpensesFromGroup);
        registry.RegisterHandler(Operation.GetExpensesFromUser, HandleGetExpensesFromUser);
        _expenseRepositoryService = expenseRepositoryService;
    }
    
    public string ProcessRequest(Operation operation, object data)
    {
        return registry.HandleRequest(operation, data);
    }
    
    private string HandleGetExpensesFromUser(object data) {
        try {
            var user = JsonConvert.DeserializeObject<User>(data.ToString());
            List<Expense> expenses = _expenseRepositoryService.GetExpenseFromUser(user);
            var response = new ApiResponse {Success = true, Data = JsonConvert.SerializeObject(groups)};
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetExpensesFromGroup(object arg) {
        
        
    }

    private string HandleCreateExpense(object data) {
        try {
            Console.WriteLine("Creating expense from data: {0}", data.ToString());
            var expense = JsonConvert.DeserializeObject<PostExpense>(data.ToString());
            Group groupAdded = _expenseRepositoryService.AddExpense(expense);
            var response = new ApiResponse {Success = true, Data = JsonConvert.SerializeObject(groupAdded)};
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    public string HandleRequest(Operation operation, object data) {
        try {
            return ProcessRequest(operation, data);
        } catch (Exception ex) {
            return JsonConvert.SerializeObject(new { error = $"Error handling request: {ex.Message}" });
        }
    }
}
