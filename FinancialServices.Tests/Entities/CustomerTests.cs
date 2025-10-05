using System;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Enums;
using Xunit;

namespace FinancialServices.Tests.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var dob = new DateTime(1990, 1, 1);
            var customer = new Customer("Rolando", dob, Sex.Male, 1200m);

            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.Equal("Rolando", customer.Name);
            Assert.Equal(dob, customer.DateOfBirth);
            Assert.Equal(Sex.Male, customer.Sex);
            Assert.Equal(1200m, customer.Income);
            Assert.NotNull(customer.Accounts);
            Assert.Empty(customer.Accounts);
        }

        [Fact]
        public void AddAccount_ShouldAddAccountToAccountsList()
        {
            var customer = new Customer("Rolando", new DateTime(1990, 1, 1), Sex.Male, 1000m);
            var account = new BankAccount("ACC-101", customer, 50m);

            customer.AddAccount(account);

            Assert.Contains(account, customer.Accounts);
            Assert.Single(customer.Accounts);
        }

        [Fact]
        public void AddAccount_ShouldThrow_WhenAccountIsNull()
        {
            var customer = new Customer("Rolando", new DateTime(1990, 1, 1), Sex.Male, 1000m);

            Assert.Throws<ArgumentNullException>(() => customer.AddAccount(null!));
        }
    }
}
