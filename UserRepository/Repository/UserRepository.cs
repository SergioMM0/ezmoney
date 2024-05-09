using Domain;

namespace UserRepository.Repository; 

public class UserRepository : IUserRepository{
    private readonly UserRepositoryContext _context;
    
    public UserRepository(UserRepositoryContext context) {
        _context = context;
        CreateDB();
    }

    public User AddUser(User user) {
         _context.UserTable.Add(user);
         _context.SaveChanges();
         return user;
    }

    public List<User> GetAllUsers() {
        return _context.UserTable.ToList();
    }

    public User LoginUser(User user) {
        return _context.UserTable.FirstOrDefault(u => u.PhoneNumber == user.PhoneNumber);
    }

    private void CreateDB() {
        _context.Database.EnsureCreated();
    }
}
