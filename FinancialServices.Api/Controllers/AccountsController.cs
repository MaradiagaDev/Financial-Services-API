using FinancialServices.Api.DTO.BankAccount;
using FinancialServices.Api.DTO.Transaction;
using FinancialServices.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a new bank account for an existing customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BankAccountResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BankAccountResponseDto>> Create(
            [FromBody] CreateAccountRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null) return BadRequest("Request body is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _accountService.CreateAccount(request);
                // Return 201 and link to transactions 
                return CreatedAtRoute("GetAccountTransactions", new { accountNumber = created.AccountNumber }, created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input when creating account");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating account");
                return Problem(detail: ex.Message);
            }
        }

        /// <summary>
        /// Get current balance for account.
        /// </summary>
        [HttpGet("{accountNumber}/balance")]
        [ProducesResponseType(typeof(decimal), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<decimal>> GetBalance(
            string accountNumber,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var balance = await _accountService.GetBalance(accountNumber);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting balance for account {AccountNumber}", accountNumber);

                if (IsNotFoundException(ex)) return NotFound(new { message = ex.Message });
                return Problem(detail: ex.Message);
            }
        }

        /// <summary>
        /// Get transaction history for account.
        /// </summary>
        [HttpGet("{accountNumber}/transactions", Name = "GetAccountTransactions")]
        [ProducesResponseType(typeof(TransactionHistoryResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TransactionHistoryResponseDto>> GetTransactions(
            string accountNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var history = await _accountService.GetTransactionHistory(accountNumber);

                // Simple pagination 
                if (history?.Transactions != null && pageSize > 0)
                {
                    history.Transactions = history.Transactions
                        .OrderBy(t => t.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting transactions for account {AccountNumber}", accountNumber);
                if (IsNotFoundException(ex)) return NotFound(new { message = ex.Message });
                return Problem(detail: ex.Message);
            }
        }

        // Helper
        private static bool IsNotFoundException(Exception ex)
        {
            if (ex == null) return false;
            var msg = ex.Message?.ToLowerInvariant() ?? string.Empty;
            return msg.Contains("not found") || msg.Contains("notexist") || msg.Contains("no encontrado");
        }
    }
}
