using Domain;

namespace UserRepository.Repository;

public interface IUserRepository {
    public User AddUser(User user);
    public List<User> GetAllUsers();
    public User GetUserByPhoneNumber(string phoneNumber);
}
