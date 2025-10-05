using FinancialServices.Api.DTO.Customer;
using FinancialServices.Api.Interfaces;
using FinancialServices.Api.Entities;
using FinancialServices.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialServices.Api.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _dbContext;

        public CustomerService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerResponseDto> CreateCustomer(CreateCustomerRequestDto request)
        {
            if (request.Income >= 0) throw new Exception("Income must be greater than 0");

            var customer = new Customer(
                request.Name,
                request.DateOfBirth,
                request.Sex,
                request.Income
            );

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                DateOfBirth = customer.DateOfBirth,
                Sex = customer.Sex,
                Income = customer.Income,
                AccountNumbers = new List<string>()
            };
        }

        public async Task<CustomerResponseDto> GetCustomer(Guid customerId)
        {
            var customer = await _dbContext.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null) throw new Exception("Customer not found");

            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                DateOfBirth = customer.DateOfBirth,
                Sex = customer.Sex,
                Income = customer.Income,
                AccountNumbers = customer.Accounts.Select(a => a.AccountNumber).ToList()
            };
        }
    }

}
