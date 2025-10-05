namespace FinancialServices.Api.Entities
{
    using FinancialServices.Api.Enums;

    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Sex Sex { get; set; }
        public decimal Income { get; set; }

        // Relaciones
        public List<BankAccount> Accounts { get; set; } = new();

        // Constructor
        public Customer(string name, DateTime dateOfBirth, Sex sex, decimal income)
        {
            Id = Guid.NewGuid();
            Name = name;
            DateOfBirth = dateOfBirth;
            Sex = sex;
            Income = income;
        }

        // Método para agregar cuenta
        public void AddAccount(BankAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            Accounts.Add(account);
        }
    }

}
