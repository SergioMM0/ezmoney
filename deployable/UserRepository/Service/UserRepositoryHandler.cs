using Domain;
using Domain.packages;
using Domain.packages.Interfaces;
using Newtonsoft.Json;

namespace UserRepository.Service;

public class UserRepositoryHandlers : IRequestHandler {
    private HandlerRegistry registry;

    private readonly UserRepositoryService _userRepositoryService;
    public UserRepositoryHandlers(UserRepositoryService userRepositoryService = null) {
        registry = new HandlerRegistry();
        registry.RegisterHandler(Operation.CreateUser, HandleCreateUser);
        registry.RegisterHandler(Operation.GetAllUsers, HandleGetAllUsers);
        registry.RegisterHandler(Operation.LoginUser, HandleLoginUser);
        _userRepositoryService = userRepositoryService;
        // Add more handlers as needed
    }

    private string HandleCreateUser(object data) {
        try {
            var user = JsonConvert.DeserializeObject<User>(data.ToString());
            User userAdded = _userRepositoryService.AddUser(user);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(userAdded) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }

    }

    private string HandleGetAllUsers(object data) {
        try {
            List<User> users = new List<User>();
            users = _userRepositoryService.GetAllUsers();
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(users) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception e) {
            var response = new ApiResponse { Success = false, ErrorMessage = e.Message };
            return JsonConvert.SerializeObject(response);
        }
    }
    private string HandleLoginUser(object data) {
        try {
            var user = JsonConvert.DeserializeObject<User>(data.ToString());
            User userLogin = _userRepositoryService.LoginUser(user);
            var response = new ApiResponse { Success = true, Data = JsonConvert.SerializeObject(userLogin) };
            return JsonConvert.SerializeObject(response);
        } catch (Exception ex) {
            var response = new ApiResponse { Success = false, ErrorMessage = ex.Message };
            return JsonConvert.SerializeObject(response);
        }



    }
    public string ProcessRequest(Operation operation, object data) {
        return registry.HandleRequest(operation, data);
    }


    public string HandleRequest(Operation operation, object data) {
        {
            try {
                switch (operation) {
                    case Operation.CreateUser:
                        return ProcessRequest(operation, data);
                    case Operation.GetAllUsers:
                        return ProcessRequest(operation, data);
                    case Operation.LoginUser:
                        return ProcessRequest(operation, data);
                    default:
                        return JsonConvert.SerializeObject(new { error = "Unknown operation" });
                }
            } catch (Exception ex) {
                return JsonConvert.SerializeObject(new { error = $"Error handling request: {ex.Message}" });
            }
        }


    }



    private string CreateUser(object data) {
        // Assume data can be deserialized into a User object
        var user = JsonConvert.DeserializeObject<User>(data.ToString());
        Console.WriteLine($"Creating user: {user.Name}");
        _userRepositoryService.AddUser(user);
        // Simulate user creation logic
        return JsonConvert.SerializeObject(user);
    }
    private string GetAllUsers() {
        List<User> users = new List<User>();
        users = _userRepositoryService.GetAllUsers();
        return JsonConvert.SerializeObject(users);
    }

}
