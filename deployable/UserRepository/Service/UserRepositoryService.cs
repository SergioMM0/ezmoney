using Domain;
using UserRepository.Repository;

namespace UserRepository.Service;

public class UserRepositoryService {
    private readonly IUserRepository _userRepository;


    public UserRepositoryService(IUserRepository userRepository) {
        _userRepository = userRepository;
    }

    public User AddUser(User user) {
        return _userRepository.AddUser(user);
    }

    public List<User> GetAllUsers() {
        return _userRepository.GetAllUsers();
    }

    public User GetUserByPhoneNumber(User user) {
        return _userRepository.GetUserByPhoneNumber(user);
    }
}
