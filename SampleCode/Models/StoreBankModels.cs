namespace StoreMobile.Models
{
    public class AttendanceRecord
    {
        public long Id { get; set; }
        public string UserIdentity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ActionType { get; set; } = string.Empty; // CheckIn or CheckOut
        public string TerminalNode { get; set; } = "PYTHON-VISION-NODE";
    }
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
    // Models/StoreBankModels.cs modification snippet
public class TransactionRecord
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string SourceWallet { get; set; } = string.Empty;
    public string DestinationWallet { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Completed";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string IntegrityHashSignature { get; set; } = string.Empty;
}
