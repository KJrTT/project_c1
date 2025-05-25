namespace BankingSystem.Models
{
    public class Bank
    {
        public int BankId { get; set; }
        public string Name { get; set; }
        public decimal DepositRate { get; set; }
        public decimal CreditFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 