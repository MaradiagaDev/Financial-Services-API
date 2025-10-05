using FinancialServices.Api.Enums;

namespace FinancialServices.Api.DTO.Customer
{
    public class CreateCustomerRequestDto
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Sex Sex { get; set; }
        public decimal Income { get; set; }
    }
}
