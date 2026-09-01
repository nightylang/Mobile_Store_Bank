namespace StoreMobile.Models
{
    public class User
    {
        // Changed from int to long to map 64-bit SQL Server BIGINT allocations
        public long Id { get; set; }
        
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Merchant"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
