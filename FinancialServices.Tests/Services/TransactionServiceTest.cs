using FinancialServices.Api.DTO.Transaction;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Enums;
using FinancialServices.Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinancialServices.Tests.Services
{
    public class TransactionServiceTests
    {
        private Customer CreateCustomer() =>
            new Customer("Rolando", new DateTime(1990, 1, 1), FinancialServices.Api.Enums.Sex.Male, 1000m);

        [Fact]
        public async Task ExecuteTransaction_Deposit_ShouldUpdateBalanceAndCreateTransaction()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            var account = new BankAccount("ACC-TX-1", customer, 100m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var req = new CreateTransactionRequestDto
            {
                Type = TransactionType.Deposit,
                Amount = 50m
            };

            var res = await service.ExecuteTransaction(account.AccountNumber, req);

            Assert.Equal(TransactionType.Deposit, res.Type);
            Assert.Equal(50m, res.Amount);

            // verificar balance en BD
            var accInDb = await context.BankAccounts.FirstAsync(a => a.Id == account.Id);
            Assert.Equal(150m, accInDb.Balance);

            // verificar transacción creada
            var tx = context.Transactions.FirstOrDefault(t => t.Id == res.Id);
            Assert.NotNull(tx);
            Assert.Equal(150m, tx!.BalanceAfterTransaction);
        }

        [Fact]
        public async Task ExecuteTransaction_Withdrawal_ShouldUpdateBalanceAndCreateTransaction()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            var account = new BankAccount("ACC-TX-2", customer, 200m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var req = new CreateTransactionRequestDto
            {
                Type = TransactionType.Withdrawal,
                Amount = 70m
            };

            var res = await service.ExecuteTransaction(account.AccountNumber, req);

            var accInDb = await context.BankAccounts.FirstAsync(a => a.Id == account.Id);
            Assert.Equal(130m, accInDb.Balance);

            var tx = context.Transactions.FirstOrDefault(t => t.Id == res.Id);
            Assert.NotNull(tx);
            Assert.Equal(TransactionType.Withdrawal, tx!.Type);
        }

        [Fact]
        public async Task ExecuteTransaction_ShouldThrow_WhenAccountNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new TransactionService(context);

            var req = new CreateTransactionRequestDto
            {
                Type = TransactionType.Deposit,
                Amount = 10m
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ExecuteTransaction("NOEX", req));
        }

        [Fact]
        public async Task ExecuteTransaction_ShouldThrow_WhenAmountInvalid()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            var account = new BankAccount("ACC-TX-3", customer, 100m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var req = new CreateTransactionRequestDto
            {
                Type = TransactionType.Deposit,
                Amount = 0m
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.ExecuteTransaction(account.AccountNumber, req));
        }
    }
}
