using Domain;
using Messages.Expense;
using Messages.Expense.Dto;
using Messages.Expense.Request;
using Newtonsoft.Json;
using RPC;
using RPC.Interfaces;

namespace ExpenseRepository.Service;

public class ExpenseRepositoryHandlers : IRequestHandler {

    private HandlerRegistry registry;

    private readonly ExpenseRepositoryService _expenseRepositoryService;

    public ExpenseRepositoryHandlers(ExpenseRepositoryService expenseRepositoryService = null) {
        registry = new HandlerRegistry();
        registry.RegisterHandler(Operation.CreateExpense, HandleCreateExpense);
        registry.RegisterHandler(Operation.GetExpensesFromGroup, HandleGetExpensesFromGroup);
        registry.RegisterHandler(Operation.GetExpensesFromUserInGroup, HandleGetExpensesFromUser);
        _expenseRepositoryService = expenseRepositoryService;
    }

    public string ProcessRequest(Operation operation, object data) {
        return registry.HandleRequest(operation, data);
    }

    private string HandleGetExpensesFromUser(object data) {
        try {
            var expenseDto = JsonConvert.DeserializeObject<GetExpensesUserReq>(data.ToString()!);
            var expenses = _expenseRepositoryService.GetExpensesFromUser(expenseDto!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(expenses) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetExpensesFromGroup(object data) {
        try {
            var expenseDto = JsonConvert.DeserializeObject<GetExpensesReq>(data.ToString()!);
            var expenses = _expenseRepositoryService.GetExpensesFromGroup(expenseDto!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(expenses) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleCreateExpense(object data) {
        try {
            var expense = JsonConvert.DeserializeObject<PostExpense>(data.ToString()!);
            var expenseAdded = _expenseRepositoryService.Create(expense!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(expenseAdded) };
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
