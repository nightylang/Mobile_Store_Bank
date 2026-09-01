using Microsoft.EntityFrameworkCore;
using MobileStoreBank.Models; // Links explicitly to your separate Models folder

namespace MobileStoreBank.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<TransactionRecord> TransactionRecords => Set<TransactionRecord>();
        public DbSet<CrmTicket> CrmTickets => Set<CrmTicket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().Property(p => p.Price).HasConversion<double>();
            modelBuilder.Entity<Wallet>().Property(w => w.Balance).HasConversion<double>();
            modelBuilder.Entity<Wallet>().Property(w => w.PendingClearance).HasConversion<double>();
            modelBuilder.Entity<TransactionRecord>().Property(t => t.Amount).HasConversion<double>();
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.Add(new User { Username = "admin", Email = "admin@storebank.com", PasswordHash = "admin123", Role = "Admin" });
            }

            if (!context.Products.Any())
            {
                context.Products.Add(new Product { Name = "iPhone 15 Pro Max", SKU = "IPH-15PM", Category = "Devices", Price = 1199.99m, Stock = 45 });
            }

            if (!context.Wallets.Any())
            {
                context.Wallets.Add(new Wallet { AssetName = "USD Core Ledger Pool", Balance = 142500.50m, PendingClearance = 3200.00m });
            }

            context.SaveChanges();
        }
    }
}
