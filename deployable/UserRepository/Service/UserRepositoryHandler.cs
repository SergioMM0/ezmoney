using Messages.User.Request;
using Newtonsoft.Json;
using RPC;
using RPC.Interfaces;

namespace UserRepository.Service;

public class UserRepositoryHandlers : IRequestHandler {
    private readonly HandlerRegistry _registry;
    private readonly UserRepositoryService _userRepositoryService;
    
    public UserRepositoryHandlers(UserRepositoryService userRepositoryService = null) {
        _registry = new HandlerRegistry();
        _registry.RegisterHandler(Operation.CreateUser, HandleCreateUser);
        _registry.RegisterHandler(Operation.GetAllUsers, HandleGetAllUsers);
        _registry.RegisterHandler(Operation.GetUserByPhoneNumber, HandleGetUserByPhoneNumber);
        _userRepositoryService = userRepositoryService;
        // Add more handlers as needed
    }

    private string HandleCreateUser(object data) {
        try {
            var user = JsonConvert.DeserializeObject<CreateUserReq>(data.ToString()!);
            var userAdded = _userRepositoryService.AddUser(user!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(userAdded) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetAllUsers(object data) {
        try {
            var users = _userRepositoryService.GetAllUsers();
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(users) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string HandleGetUserByPhoneNumber(object data) {
        try {
            var user = JsonConvert.DeserializeObject<GetUserByPhone>(data.ToString()!);
            var userWithPhoneNumber = _userRepositoryService.GetUserByPhoneNumber(user!);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(userWithPhoneNumber) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception ex) {
            var response = new ApiResponse { Success = false, ErrorMessage = ex.Message };
            return JsonConvert.SerializeObject(response);
        }
    }

    private string ProcessRequest(Operation operation, object data) {
        return _registry.HandleRequest(operation, data);
    }

    public string HandleRequest(Operation operation, object data) {
        try {
            switch (operation) {
                case Operation.CreateUser:
                    return ProcessRequest(operation, data);
                case Operation.GetAllUsers:
                    return ProcessRequest(operation, data);
                case Operation.GetUserByPhoneNumber:
                    return ProcessRequest(operation, data);
                default:
                    return JsonConvert.SerializeObject(new { error = "Unknown operation" });
            }
        } catch (Exception ex) {
            return JsonConvert.SerializeObject(new { error = $"Error handling request: {ex.Message}" });
        }
    }
}
