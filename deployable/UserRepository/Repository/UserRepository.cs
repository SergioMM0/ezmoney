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
            Monitoring.Monitoring.Log.Information($"User added: {user.Id} {user.Name} {user.PhoneNumber}");
            return user;
        } catch (Exception ex) {
            Monitoring.Monitoring.Log.Error("An error occurred while adding the user: " + ex.Message);
            throw new ApplicationException("An error occurred while adding the user.", ex);
        }

    }

    public List<User> GetAllUsers() {
        try {
            return _context.UserTable.ToList();
        } catch (Exception ex) {
            Monitoring.Monitoring.Log.Error("An error occurred while getting the users: " + ex.Message);
            throw new ApplicationException("An error occurred while adding the user.", ex);
        }

    }

    public User GetUserByPhoneNumber(string phoneNumber) {
        try {
            return _context.UserTable.FirstOrDefault(u => u.PhoneNumber == phoneNumber)!;
        } catch (Exception ex) {
            Monitoring.Monitoring.Log.Error("An error occurred while retrieving the user: " + ex.Message);
            throw new ApplicationException("An error occurred while retrieving the user.", ex);
        }
    }

    private void CreateDB() {
        _context.Database.EnsureCreated();
    }
}
