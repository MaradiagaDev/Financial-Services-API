using System;
using FinancialServices.Api;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Enums;
using Xunit;

namespace FinancialServices.Tests.Entities
{
    public class BankAccountTests
    {
        private Customer CreateCustomer() =>
            new Customer("Rolando", new DateTime(1990, 1, 1), Sex.Male, 1000m);

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-001", customer, 100m);

            Assert.NotEqual(Guid.Empty, account.Id);
            Assert.Equal("ACC-001", account.AccountNumber);
            Assert.Equal(customer.Id, account.CustomerId);
            Assert.Same(customer, account.Customer);
            Assert.Equal(100m, account.Balance);
            Assert.True((DateTime.UtcNow - account.CreatedAt) < TimeSpan.FromSeconds(5));
            Assert.NotNull(account.Transactions);
        }

        [Fact]
        public void CannotCreateAccount_WithNegativeInitialBalance()
        {
            var customer = CreateCustomer();
            Assert.Throws<ArgumentException>(() => new BankAccount("ACC-002", customer, -1m));
        }

        [Fact]
        public void Deposit_ShouldIncreaseBalance()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-003", customer, 100m);

            account.Deposit(50m);

            Assert.Equal(150m, account.Balance);
        }

        [Fact]
        public void Deposit_ShouldThrow_WhenAmountIsNotPositive()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-004", customer, 100m);

            Assert.Throws<ArgumentException>(() => account.Deposit(0m));
            Assert.Throws<ArgumentException>(() => account.Deposit(-10m));
        }

        [Fact]
        public void Withdraw_ShouldDecreaseBalance()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-005", customer, 200m);

            account.Withdraw(50m);

            Assert.Equal(150m, account.Balance);
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenAmountIsNotPositive()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-006", customer, 200m);

            Assert.Throws<ArgumentException>(() => account.Withdraw(0m));
            Assert.Throws<ArgumentException>(() => account.Withdraw(-5m));
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenInsufficientFunds()
        {
            var customer = CreateCustomer();
            var account = new BankAccount("ACC-007", customer, 50m);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(100m));
        }
    }
}
