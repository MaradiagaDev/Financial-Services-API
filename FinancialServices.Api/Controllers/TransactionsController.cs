using FinancialServices.Api.DTO.Transaction;
using FinancialServices.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialServices.Api.Controllers
{
    [ApiController]
    [Route("api/accounts/{accountNumber}/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Execute a deposit or withdrawal on the specified account.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TransactionResponseDto>> Execute(
            string accountNumber,
            [FromBody] CreateTransactionRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(accountNumber)) return BadRequest(new { message = "Account number is required." });
            if (request == null) return BadRequest(new { message = "Request body is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.ExecuteTransaction(accountNumber, request);

                // Return 201 Created and point to the account transactions collection
                return CreatedAtRoute("GetAccountTransactions", new { accountNumber = accountNumber }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for transaction on account {Account}", accountNumber);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // used for domain rules like "Insufficient funds"
            {
                _logger.LogInformation(ex, "Domain rule prevented transaction on account {Account}: {Reason}", accountNumber, ex.Message);
                // For insufficient funds or similar business-rule rejections, return 400 with reason
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing transaction for account {Account}", accountNumber);
                if (IsNotFoundException(ex)) return NotFound(new { message = ex.Message });
                return Problem(detail: ex.Message);
            }
        }

        // Helper: detect "not found" style exceptions. Replace with typed exceptions in production.
        private static bool IsNotFoundException(Exception ex)
        {
            if (ex == null) return false;
            var msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
            return msg.Contains("not found") || msg.Contains("notexist") || msg.Contains("no encontrado");
        }
    }
}
