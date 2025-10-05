using FinancialServices.Api.DTO.Customer;

namespace FinancialServices.Api.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomer(CreateCustomerRequestDto request);
        Task<CustomerResponseDto> GetCustomer(Guid customerId);
    }
}
