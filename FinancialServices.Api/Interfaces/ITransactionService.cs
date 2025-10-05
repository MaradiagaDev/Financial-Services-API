using FinancialServices.Api.DTO.Transaction;

namespace FinancialServices.Api.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> ExecuteTransaction(string accountNumber, CreateTransactionRequestDto request);
    }
}
