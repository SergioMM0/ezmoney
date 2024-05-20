using Domain;
using Messages.User.Request;
using Messages.User.Response;
using UserRepository.Repository;

namespace UserRepository.Service;

public class UserRepositoryService {
    private readonly IUserRepository _userRepository;


    public UserRepositoryService(IUserRepository userRepository) {
        _userRepository = userRepository;
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
