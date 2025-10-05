using FinancialServices.Api.Enums;

namespace FinancialServices.Api.DTO.Transaction
{
    public class CreateTransactionRequestDto
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
