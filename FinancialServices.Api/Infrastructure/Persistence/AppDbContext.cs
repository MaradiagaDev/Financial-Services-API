namespace FinancialServices.Api.Infrastructure.Persistence
{
    using FinancialServices.Api.Entities;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Accounts)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BankAccount>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<BankAccount>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<BankAccount>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);
        }
    }
}
