using Microsoft.EntityFrameworkCore;

namespace StoreMobile.Data
{
    // ==========================================
    // DOMAIN ENTITY CORE LAYER MODELS
    // ==========================================

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Merchant"; // Admin or Merchant
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class Wallet
    {
        public int Id { get; set; }
        public string WalletAddress { get; set; } = Guid.NewGuid().ToString("N");
        public string AssetName { get; set; } = "USD";
        public decimal Balance { get; set; }
        public decimal PendingClearance { get; set; }
    }

    public class TransactionRecord
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string SourceWallet { get; set; } = string.Empty;
        public string DestinationWallet { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CrmTicket
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string IssueSummary { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
        public string Status { get; set; } = "Open";     // Open, In-Progress, Closed
    }

    // ==========================================
    // ENTITY FRAMEWORK CORE 10 DATA PERSISTENCE
    // ==========================================

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

            // SQLite optimization mappings (converting decimal properties to numeric/double representation)
            modelBuilder.Entity<Product>().Property(p => p.Price).HasConversion<double>();
            modelBuilder.Entity<Wallet>().Property(w => w.Balance).HasConversion<double>();
            modelBuilder.Entity<Wallet>().Property(w => w.PendingClearance).HasConversion<double>();
            modelBuilder.Entity<TransactionRecord>().Property(t => t.Amount).HasConversion<double>();
        }
    }

    // ==========================================
    // SEED DATA INITIALIZATION AUTOMATION ENGINE
    // ==========================================

    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database file schema gets allocated safely on startup
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Username = "admin", Email = "admin@storebank.com", PasswordHash = "admin123", Role = "Admin" },
                    new User { Username = "merchant", Email = "merchant@storebank.com", PasswordHash = "password123", Role = "Merchant" }
                );
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "iPhone 15 Pro Max", SKU = "IPH-15PM", Category = "Devices", Price = 1199.99m, Stock = 45 },
                    new Product { Name = "SaaS Settlement Micro-Gateway Token", SKU = "SAAS-V2", Category = "SaaS Licenses", Price = 89.00m, Stock = 1000 },
                    new Product { Name = "Wireless POS Terminal X", SKU = "POS-X4", Category = "Hardware", Price = 299.50m, Stock = 15 }
                );
            }

            if (!context.Wallets.Any())
            {
                context.Wallets.AddRange(
                    new Wallet { AssetName = "USD Core Ledger Pool", Balance = 142500.50m, PendingClearance = 3200.00m },
                    new Wallet { AssetName = "BTC Cold Settlement Vault", Balance = 1.425m, PendingClearance = 0.05m }
                );
            }

            if (!context.TransactionRecords.Any())
            {
                context.TransactionRecords.AddRange(
                    new TransactionRecord { ReferenceNumber = "TXN-001", SourceWallet = "External Counterparty", DestinationWallet = "USD Core Ledger Pool", Amount = 1199.99m },
                    new TransactionRecord { ReferenceNumber = "TXN-002", SourceWallet = "USD Core Ledger Pool", DestinationWallet = "External Node", Amount = 450.00m }
                );
            }

            if (!context.CrmTickets.Any())
            {
                context.CrmTickets.AddRange(
                    new CrmTicket { CustomerName = "Phnom Penh Retail Group", IssueSummary = "API Webhook payload tracking delay over HTTP channel bindings", Priority = "High", Status = "Open" },
                    new CrmTicket { CustomerName = "Global Logistics Node", IssueSummary = "Batch daily transaction verification review request", Priority = "Medium", Status = "In-Progress" }
                );
            }

            context.SaveChanges();
        }
    }
}
