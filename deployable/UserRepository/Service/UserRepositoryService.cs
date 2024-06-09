using Domain;
using Messages.User.Request;
using Messages.User.Response;
using OpenTelemetry.Trace;
using UserRepository.Repository;

namespace UserRepository.Service;

public class UserRepositoryService {
    private readonly IUserRepository _userRepository;
    private readonly Tracer _tracer;


    public UserRepositoryService(IUserRepository userRepository, Tracer tracer) {
        _userRepository = userRepository;
        _tracer = tracer;
    }
    
    public List<UserResponse> GetAllUsers() {
        return _userRepository.GetAllUsers()
            .Select(user => new UserResponse() {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            }).ToList();
    }

    public UserResponse GetUserByPhoneNumber(GetUserByPhone request) {
        var user = _userRepository.GetUserByPhoneNumber(request.PhoneNumber);
        
        return new UserResponse() {
            Id = user.Id,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber
        };
    }

    public UserResponse AddUser(CreateUserReq request) {
        using var activity = _tracer.StartActiveSpan("AddUser");
        var user = new User() {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber
        };

        var created = _userRepository.AddUser(user);

        return new UserResponse() {
            Id = created.Id,
            Name = created.Name,
            PhoneNumber = created.PhoneNumber
        };
    }
}
