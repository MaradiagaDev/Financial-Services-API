using FinancialServices.Api.Enums;

namespace FinancialServices.Api.DTO.Customer
{
    public class CustomerResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Sex Sex { get; set; }
        public decimal Income { get; set; }
        public List<string> AccountNumbers { get; set; } = new();
    }
}
