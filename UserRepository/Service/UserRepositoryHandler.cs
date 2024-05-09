using Domain;
using Domain.packages;
using Domain.packages.Interfaces;
using Newtonsoft.Json;

namespace UserRepository.Service; 

public class UserRepositoryHandlers : IRequestHandler
{
    private HandlerRegistry registry;

    private readonly UserRepositoryService _userRepositoryService;
    public UserRepositoryHandlers(UserRepositoryService userRepositoryService=null)
    {
        registry = new HandlerRegistry();
        registry.RegisterHandler(Operation.CreateUser, HandleCreateUser);
        registry.RegisterHandler(Operation.GetAllUsers, HandleGetAllUsers);
        registry.RegisterHandler(Operation.LoginUser, HandleLoginUser);
        _userRepositoryService = userRepositoryService;
        // Add more handlers as needed
    }
    
    private string HandleCreateUser(object data)
    {
        var user = JsonConvert.DeserializeObject<User>(data.ToString());
        Console.WriteLine($"Creating user: {user.Name}");
        _userRepositoryService.AddUser(user);
        // Simulate user creation logic
        return JsonConvert.SerializeObject(user);
    }

    private string HandleGetAllUsers(object data)
    {
        List<User> users = new List<User>();
        users = _userRepositoryService.GetAllUsers();
        return JsonConvert.SerializeObject(users);
    }
    private string HandleLoginUser(object data)
    {
        var user = JsonConvert.DeserializeObject<User>(data.ToString());
        Console.WriteLine($"Logging in user: {user.PhoneNumber}");
        User userLogin = _userRepositoryService.LoginUser(user);
       
        return JsonConvert.SerializeObject(userLogin);
    }
    public string ProcessRequest(Operation operation, object data)
    {
        return registry.HandleRequest(operation, data);
    }


    public string HandleRequest(Operation operation, object data) {
        {
            try
            {
                switch (operation)
                {
                    case Operation.CreateUser:
                        return ProcessRequest(operation, data);
                    case Operation.GetAllUsers:
                        return ProcessRequest(operation, data);
                    case Operation.LoginUser:
                        return ProcessRequest(operation, data);
                    default:
                        return JsonConvert.SerializeObject(new { error = "Unknown operation" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = $"Error handling request: {ex.Message}" });
            }
        }

       
    }

    

    private string CreateUser(object data)
    {
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
