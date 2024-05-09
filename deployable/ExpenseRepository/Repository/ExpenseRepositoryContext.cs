using Domain;
using Microsoft.EntityFrameworkCore;

namespace ExpenseRepository.Repository; 

public class ExpenseRepositoryContext : DbContext {
    public ExpenseRepositoryContext(DbContextOptions<ExpenseRepositoryContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<UserExpense>()
            .HasKey(ue => new {ue.UserId, ue.ExpenseId});
    }
    DbSet<Expense> ExpenseTable { get; set; }
    DbSet<UserExpense> UserExpenseTable { get; set; }
}
