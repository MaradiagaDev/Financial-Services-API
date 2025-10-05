using FinancialServices.Api.DTO.Customer;
using FinancialServices.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        /// <summary>
        /// Create a new customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CustomerResponseDto>> Create(
            [FromBody] CreateCustomerRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) return BadRequest("Request body is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _customerService.CreateCustomer(request);
                // Returns 201 Created with Location header for GET by id
                return CreatedAtRoute("GetCustomerById", new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                // Bad input from client
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // For unexpected errors, you may prefer to let a global middleware handle it.
                return Problem(detail: ex.Message);
            }
        }

        /// <summary>
        /// Get customer by id.
        /// </summary>
        [HttpGet("{id:guid}", Name = "GetCustomerById")]
        [ProducesResponseType(typeof(CustomerResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CustomerResponseDto>> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerService.GetCustomer(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                // Prefer using a custom NotFoundException and global error handling
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
