using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileIdentityApi.Services;
using System.Security.Claims;

namespace MobileIdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var kycId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kycId))
            {
                _logger.LogWarning("Unauthorized access attempt: missing NameIdentifier claim");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            try
            {
                var customerInfo = await _customerService.GetCustomerInfoAsync(kycId);

                if (customerInfo == null)
                {
                    _logger.LogWarning("Customer info not found for KycId {kycId}", kycId);
                    return NotFound(new { error = "Customer info not found" });
                }

                return Ok(new
                {
                    FullName = customerInfo.FullName,
                    customerInfo.GenderId,
                    customerInfo.MaritalStatusId,
                    customerInfo.DateOfBirth
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer info for KycId {kycId}", kycId);
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpGet("account-details")]
        [Authorize]
        public async Task<IActionResult> GetAccountDetails()
        {
            var kycId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kycId))
            {
                _logger.LogWarning("Unauthorized access attempt: missing NameIdentifier claim");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            try
            {
                var accountDetails = await _customerService.GetAccountDetailAsync(kycId);
                if (accountDetails == null)
                {
                    _logger.LogWarning("Account details not found for KycId {kycId}", kycId);
                    return NotFound(new { error = "Account details not found" });
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account details for KycId {kycId}", kycId);
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

        [HttpGet("{customerId}/accounts")]
        [Authorize]
        public async Task<IActionResult> GetAccountsByCustomerId(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                _logger.LogWarning("GetAccountsByCustomerId called with empty customerId");
                return BadRequest(new { error = "Customer ID is required" });
            }

            var userCustomerId = User.FindFirstValue("CustomerId");
            if (userCustomerId != customerId)
            {
                _logger.LogWarning("Unauthorized access attempt for CustomerId {customerId}", 
                    customerId);
                return Unauthorized(new { error = "Not authorized" });
            }

            try
            {
                var accounts = await _customerService.GetAccountsByCustomerIdAsync(customerId);

                if (accounts == null || !accounts.Any())
                {
                    _logger.LogWarning("No accounts found for CustomerId {customerId}", customerId);
                    return NotFound(new { error = "No accounts found for this customer" });
                }

                _logger.LogInformation("Retrieved {count} accounts for CustomerId {customerId}",
                    accounts.Count(), customerId);

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accounts for CustomerId {customerId}",
                    customerId);
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }

    }
}
