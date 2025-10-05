namespace FinancialServices.Api.DTO.BankAccount
{
    public class CreateAccountRequestDto
    {
        public Guid CustomerId { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
