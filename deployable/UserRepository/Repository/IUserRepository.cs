using Domain;

namespace UserRepository.Repository;

public interface IUserRepository {
    public Domain.User AddUser(Domain.User user);

    public List<User> GetAllUsers();
    public User GetUserByPhoneNumber(User user);
}
