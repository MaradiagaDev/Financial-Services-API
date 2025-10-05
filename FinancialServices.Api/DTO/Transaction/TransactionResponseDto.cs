using FinancialServices.Api.Enums;

namespace FinancialServices.Api.DTO.Transaction
{
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
