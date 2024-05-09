using Domain;
using Microsoft.EntityFrameworkCore;

namespace UserRepository.Repository; 

public class UserRepositoryContext : DbContext{
    public UserRepositoryContext(DbContextOptions<UserRepositoryContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
    public DbSet<User> UserTable { get; set; }
}
