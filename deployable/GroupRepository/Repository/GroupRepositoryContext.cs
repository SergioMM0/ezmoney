using Domain;
using Microsoft.EntityFrameworkCore;

namespace GroupRepository.Repository;

public class GroupRepositoryContext : DbContext {
    public GroupRepositoryContext(DbContextOptions<GroupRepositoryContext> options) : base(options) {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Group>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });
    }
    public DbSet<Group> GroupTable { get; set; }
    public DbSet<UserGroup> UserGroupTable { get; set; }
}
