namespace FinancialServices.Api.Entities
{
    using FinancialServices.Api.Enums;

    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public BankAccount Account { get; set; } = null!;
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public DateTime CreatedAt { get; set; }

        // Constructor
        public Transaction(BankAccount account, TransactionType type, decimal amount, decimal balanceAfter)
        {
            Id = Guid.NewGuid();
            AccountId = account.Id;
            Account = account;
            Type = type;
            Amount = amount;
            BalanceAfterTransaction = balanceAfter;
            CreatedAt = DateTime.UtcNow;
        }
        protected Transaction() { }
    }
}
