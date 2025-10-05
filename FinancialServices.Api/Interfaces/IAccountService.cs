using FinancialServices.Api.DTO.BankAccount;
using FinancialServices.Api.DTO.Transaction;

namespace FinancialServices.Api.Interfaces
{
    public interface IAccountService
    {
        Task<BankAccountResponseDto> CreateAccount(CreateAccountRequestDto request);
        Task<decimal> GetBalance(string accountNumber);
        Task<TransactionHistoryResponseDto> GetTransactionHistory(string accountNumber);
    }
}
