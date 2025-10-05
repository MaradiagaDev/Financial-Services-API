    namespace FinancialServices.Api.Entities
{
    using FinancialServices.Api.Enums;

    public class BankAccount
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; set; }

        // Relaciones
        public List<Transaction> Transactions { get; set; } = new();

        // Constructor
        public BankAccount(string accountNumber, Customer customer, decimal initialBalance)
        {
            Id = Guid.NewGuid();
            AccountNumber = accountNumber;
            Customer = customer;
            CustomerId = customer.Id;
            Balance = initialBalance >= 0 ? initialBalance : throw new ArgumentException("Initial balance cannot be negative");
            CreatedAt = DateTime.UtcNow;
        }

        protected BankAccount() { }

        // Método para depositar
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Deposit amount must be positive");
            Balance += amount;
        }

        // Método para retirar
        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive");
            if (amount > Balance) throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
        }
    }

}
