using FinancialServices.Api.DTO.Transaction;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Enums;
using FinancialServices.Api.Infrastructure.Persistence;
using FinancialServices.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FinancialServices.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _dbContext;

        public TransactionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TransactionResponseDto> ExecuteTransaction(string accountNumber, CreateTransactionRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            accountNumber = accountNumber.Trim();

            var account = await _dbContext.BankAccounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new KeyNotFoundException("Account not found");

            // Validar y aplicar la transacción
            switch (request.Type)
            {
                case TransactionType.Deposit:
                    account.Deposit(request.Amount);
                    break;
                case TransactionType.Withdrawal:
                    account.Withdraw(request.Amount);
                    break;
                default:
                    throw new ArgumentException("Invalid transaction type");
            }

            _dbContext.BankAccounts.Update(account);
            await _dbContext.SaveChangesAsync();

            // Crear la transacción en EF
            var transaction = new Transaction(account, request.Type, request.Amount, account.Balance);
            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Amount,
                BalanceAfterTransaction = transaction.BalanceAfterTransaction,
                CreatedAt = transaction.CreatedAt
            };
        }


    }
}
