using System;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Enums;
using Xunit;

namespace FinancialServices.Tests.Entities
{
    public class TransactionTests
    {
        private Customer CreateCustomer() =>
            new Customer("Rolando", new DateTime(1990, 1, 1), Sex.Male, 1000m);

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-201", customer, 500m);

            var tx = new Transaction(account, TransactionType.Deposit, 100m, 600m);

            Assert.NotEqual(Guid.Empty, tx.Id);
            Assert.Equal(account.Id, tx.AccountId);
            Assert.Same(account, tx.Account);
            Assert.Equal(TransactionType.Deposit, tx.Type);
            Assert.Equal(100m, tx.Amount);
            Assert.Equal(600m, tx.BalanceAfterTransaction);
            Assert.True((DateTime.UtcNow - tx.CreatedAt) < TimeSpan.FromSeconds(5));
        }
    }
}
