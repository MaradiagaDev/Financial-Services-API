using System;
using System.Linq;
using System.Threading.Tasks;
using FinancialServices.Api.DTO.BankAccount;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialServices.Tests.Services
{
    public class AccountServiceTests
    {
        private Customer CreateCustomer() =>
            new Customer("Rolando", new DateTime(1990, 1, 1), FinancialServices.Api.Enums.Sex.Male, 1000m);

        [Fact]
        public async Task CreateAccount_ShouldCreateAndReturnDto()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var service = new AccountService(context);

            var req = new CreateAccountRequestDto
            {
                CustomerId = customer.Id,
                InitialBalance = 500m
            };

            var result = await service.CreateAccount(req);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(customer.Id, result.CustomerId);
            Assert.Equal(500m, result.Balance);

            // verificar que quedó en la BD
            var accInDb = await context.BankAccounts.FindAsync(result.Id);
            Assert.NotNull(accInDb);
            Assert.Equal(result.AccountNumber, accInDb!.AccountNumber);
        }

        [Fact]
        public async Task CreateAccount_ShouldThrow_WhenCustomerNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new AccountService(context);

            var req = new CreateAccountRequestDto
            {
                CustomerId = Guid.NewGuid(),
                InitialBalance = 100m
            };

            await Assert.ThrowsAsync<Exception>(() => service.CreateAccount(req));
        }

        [Fact]
        public async Task GetBalance_ShouldReturnBalance_WhenExists()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            var account = new BankAccount("ACC-01", customer, 250m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new AccountService(context);

            var balance = await service.GetBalance(account.AccountNumber);

            Assert.Equal(250m, balance);
        }

        [Fact]
        public async Task GetBalance_ShouldThrow_WhenNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new AccountService(context);

            await Assert.ThrowsAsync<Exception>(() => service.GetBalance("NO_EXISTE"));
        }

        [Fact]
        public async Task GetTransactionHistory_ShouldReturnTransactions()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = CreateCustomer();
            var account = new BankAccount("ACC-HIST", customer, 100m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            // crear transacciones manualmente
            var tx1 = new Transaction(account, FinancialServices.Api.Enums.TransactionType.Deposit, 50m, 150m);
            var tx2 = new Transaction(account, FinancialServices.Api.Enums.TransactionType.Withdrawal, 20m, 130m);
            context.Transactions.AddRange(tx1, tx2);
            await context.SaveChangesAsync();

            var service = new AccountService(context);

            var history = await service.GetTransactionHistory(account.AccountNumber);

            Assert.Equal(account.AccountNumber, history.AccountNumber);
            Assert.Equal(account.Balance, history.CurrentBalance);
            Assert.Equal(2, history.Transactions.Count);
            Assert.Equal(tx1.Id, history.Transactions.OrderBy(t => t.CreatedAt).First().Id);
        }
    }
}

