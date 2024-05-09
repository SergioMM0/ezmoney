using Domain;
using Domain.DTO.Expense;
using Domain.DTO.Group;
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
        registry.RegisterHandler(Operation.GetExpensesFromUserInGroup, HandleGetExpensesFromUserInGroup);
        _expenseRepositoryService = expenseRepositoryService;
    }
    
    public string ProcessRequest(Operation operation, object data)
    {
        return registry.HandleRequest(operation, data);
    }
    
    private string HandleGetExpensesFromUserInGroup(object data) {
        try {
            var expenseDto = JsonConvert.DeserializeObject<ExpenseDTO>(data.ToString());
            List<Expense> expenses = _expenseRepositoryService.GetExpenseFromUserInGroup(expenseDto);
            var response = new ApiResponse {Success = true, Data = JsonConvert.SerializeObject(expenses)};
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetExpensesFromGroup(object data) {
        try {
            var expenseDto = JsonConvert.DeserializeObject<ExpenseDTO>(data.ToString());
            List<Expense> expenses = _expenseRepositoryService.GetExpensesFromGroup(expenseDto);
            var response = new ApiResponse {Success = true, Data = JsonConvert.SerializeObject(expenses)};
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
        
    }

    private string HandleCreateExpense(object data) {
        try {
            Console.WriteLine("Creating expense from data: {0}", data.ToString());
            var expense = JsonConvert.DeserializeObject<PostExpense>(data.ToString());
            Expense expenseAdded = _expenseRepositoryService.AddExpense(expense);
            var response = new ApiResponse {Success = true, Data = JsonConvert.SerializeObject(expenseAdded)};
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
