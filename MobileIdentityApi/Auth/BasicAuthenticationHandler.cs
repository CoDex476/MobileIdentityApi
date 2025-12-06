using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MobileIdentityApi.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace MobileIdentityApi.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<BasicAuthenticationHandler> _logger;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ICustomerService customerService)
            : base(options, loggerFactory, encoder, clock)
        {
            _customerService = customerService;
            _logger = loggerFactory.CreateLogger<BasicAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? failureMessage = null;

            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    failureMessage = "Missing Authorization header";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                string authenticationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(authenticationHeader))
                {
                    failureMessage = "Empty Authorization header";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                if (!authenticationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    failureMessage = "Invalid Authorization scheme";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                string encodedCredentials = authenticationHeader.Substring("Basic ".Length).Trim();
                string decodedCredentials;

                try
                {
                    var bytes = Convert.FromBase64String(encodedCredentials);
                    decodedCredentials = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    failureMessage = "Invalid Base64 credentials";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                var parts = decodedCredentials.Split(':');
                if (parts.Length < 1)
                {
                    failureMessage = "Invalid credentials format";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                string phoneNumber = parts[0];
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    failureMessage = "Phone number missing";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                var customerDetails = await _customerService.GetCustomerByPhoneAsync(phoneNumber);
                if (customerDetails == null)
                {
                    failureMessage = $"Phone number not found: {phoneNumber}";
                    _logger.LogWarning(failureMessage);
                    return AuthenticateResult.Fail("Invalid credentials");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, customerDetails.KycId),
                    new Claim("CustomerId", customerDetails.CustomerId)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);

                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in BasicAuthenticationHandler");
                return AuthenticateResult.Fail("Invalid credentials");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = "application/json";

            var body = "{\"error\": \"Invalid credentials\"}";
            return Response.WriteAsync(body);
        }
    }
}
