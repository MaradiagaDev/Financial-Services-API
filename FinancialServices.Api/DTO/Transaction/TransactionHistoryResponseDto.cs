namespace FinancialServices.Api.DTO.Transaction
{
    public class TransactionHistoryResponseDto
    {
        public string AccountNumber { get; set; } = null!;
        public decimal CurrentBalance { get; set; }
        public List<TransactionResponseDto> Transactions { get; set; } = new();
    }
}
