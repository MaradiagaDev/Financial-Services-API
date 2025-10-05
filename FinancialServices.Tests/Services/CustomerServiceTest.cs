using System;
using System.Linq;
using System.Threading.Tasks;
using FinancialServices.Api.DTO.Customer;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Services;
using Xunit;

namespace FinancialServices.Tests.Services
{
    public class CustomerServiceTests
    {
        [Fact]
        public async Task CreateCustomer_ShouldCreateAndReturnDto_WhenIncomeValid()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new CustomerService(context);

            var req = new CreateCustomerRequestDto
            {
                Name = "Rolando",
                DateOfBirth = new DateTime(1990, 1, 1),
                Sex = FinancialServices.Api.Enums.Sex.Male,
                Income = 1000m
            };

            var res = await service.CreateCustomer(req);

            Assert.NotEqual(Guid.Empty, res.Id);
            Assert.Equal("Rolando", res.Name);

            // verificar en BD
            var inDb = context.Customers.FirstOrDefault(c => c.Id == res.Id);
            Assert.NotNull(inDb);
            Assert.Equal(1000m, inDb!.Income);
        }

        [Fact]
        public async Task CreateCustomer_ShouldThrow_WhenIncomeInvalid()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new CustomerService(context);

            var req = new CreateCustomerRequestDto
            {
                Name = "Bad",
                DateOfBirth = new DateTime(1990, 1, 1),
                Sex = FinancialServices.Api.Enums.Sex.Male,
                Income = 0m // <= 0 debe fallar
            };

            await Assert.ThrowsAsync<Exception>(() => service.CreateCustomer(req));
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnDto_WhenExists()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var customer = new Customer("R", new DateTime(1990, 1, 1), FinancialServices.Api.Enums.Sex.Male, 500m);
            var account = new BankAccount("ACC-X", customer, 50m);
            context.Customers.Add(customer);
            context.BankAccounts.Add(account);
            await context.SaveChangesAsync();

            var service = new CustomerService(context);

            var res = await service.GetCustomer(customer.Id);

            Assert.Equal(customer.Id, res.Id);
            Assert.Single(res.AccountNumbers);
            Assert.Equal("ACC-X", res.AccountNumbers.First());
        }

        [Fact]
        public async Task GetCustomer_ShouldThrow_WhenNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = TestHelpers.CreateContext(dbName);

            var service = new CustomerService(context);

            await Assert.ThrowsAsync<Exception>(() => service.GetCustomer(Guid.NewGuid()));
        }
    }
}
