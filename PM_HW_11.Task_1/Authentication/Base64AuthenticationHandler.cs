using System;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DepsWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DepsWebApp.Authentication
{
    /// <summary>
    /// Custom scheme handler
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Base64AuthenticationHandler:AuthenticationHandler<Base64SchemeOptions>
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="logger">logger for logging</param>
        /// <param name="encoder">encoded</param>
        /// <param name="clock">clock time</param>
        /// <param name="accountService">account database</param>
        public Base64AuthenticationHandler
            (IOptionsMonitor<Base64SchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            IAccountService accountService) 
            : base(options, logger, encoder, clock)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Handling authentication
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!GetAccountFromRequest(Request, out var encodedString))
                return AuthenticateResult.NoResult();
            try
            {
                if (!await _accountService.GetAccount(encodedString))
                    throw new AuthenticationException(encodedString);
                return AuthenticateResult.Success(
                        new AuthenticationTicket(
                            new ClaimsPrincipal(
                                new AccountIdentity(encodedString)),
                            Base64Scheme.Name));
            }
            catch (AuthenticationException exception)
            {
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                Logger.LogError(exception, exception.Message);
                return AuthenticateResult.Fail(exception);
            }
        }

        private static bool GetAccountFromRequest(HttpRequest request, out string encodedString)
        {
            encodedString = "";
            if (!request.Headers.ContainsKey(HeaderNames.Authorization)) return false;
            try
            {
                var authHeader =
                    AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
                if (!authHeader.Scheme.Equals(Base64Scheme.Name,StringComparison.OrdinalIgnoreCase))
                    return false;
                encodedString = authHeader.Parameter;
                return !string.IsNullOrEmpty(encodedString);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}