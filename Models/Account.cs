namespace BankingSystem.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int ClientId { get; set; }
        public decimal Balance { get; set; }
        public decimal InterestRate { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 