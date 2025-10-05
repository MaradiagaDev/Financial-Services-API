using FinancialServices.Api.DTO.BankAccount;
using FinancialServices.Api.DTO.Transaction;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Infrastructure.Persistence;
using FinancialServices.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialServices.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _dbContext;

        public AccountService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BankAccountResponseDto> CreateAccount(CreateAccountRequestDto request)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId);

            if (customer == null) throw new Exception("Customer not found");

            string accountNumber = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();

            var account = new BankAccount(accountNumber, customer, request.InitialBalance);

            _dbContext.BankAccounts.Add(account);
            await _dbContext.SaveChangesAsync();

            return new BankAccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                CustomerId = account.CustomerId,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<decimal> GetBalance(string accountNumber)
        {
            var account = await _dbContext.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null) throw new Exception("Account not found");

            return account.Balance;
        }

        public async Task<TransactionHistoryResponseDto> GetTransactionHistory(string accountNumber)
        {
            var account = await _dbContext.BankAccounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null) throw new Exception("Account not found");

            return new TransactionHistoryResponseDto
            {
                AccountNumber = account.AccountNumber,
                CurrentBalance = account.Balance,
                Transactions = account.Transactions
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TransactionResponseDto
                    {
                        Id = t.Id,
                        Type = t.Type,
                        Amount = t.Amount,
                        BalanceAfterTransaction = t.BalanceAfterTransaction,
                        CreatedAt = t.CreatedAt
                    }).ToList()
            };
        }
    }

}
