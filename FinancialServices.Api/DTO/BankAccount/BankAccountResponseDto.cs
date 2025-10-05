namespace FinancialServices.Api.DTO.BankAccount
{
    public class BankAccountResponseDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
