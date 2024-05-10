using Domain;

namespace UserRepository.Repository;

public class UserRepository : IUserRepository {
    private readonly UserRepositoryContext _context;

    public UserRepository(UserRepositoryContext context) {
        _context = context;
        CreateDB();
    }

    public User AddUser(User user) {
        try {
            _context.UserTable.Add(user);
            _context.SaveChanges();
            return user;
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the user.", ex);
        }

    }

    public List<User> GetAllUsers() {
        try {
            return _context.UserTable.ToList();
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while adding the user.", ex);
        }

    }

    public User LoginUser(User user) {
        try {
            return _context.UserTable.FirstOrDefault(u => u.PhoneNumber == user.PhoneNumber);
        } catch (Exception ex) {
            throw new ApplicationException("An error occurred while retrieving the user.", ex);
        }
    }

    private void CreateDB() {
        _context.Database.EnsureCreated();
    }
}
