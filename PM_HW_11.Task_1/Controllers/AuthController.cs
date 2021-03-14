using System;
using System.Net;
using System.Threading.Tasks;
using DepsWebApp.Middlewares;
using DepsWebApp.Models;
using DepsWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Authenticator controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthController(IAccountService accountService,
            ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }
        
        /// <summary>
        /// Method to register a new account
        /// </summary>
        /// <param name="account">Account credentials</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [TypeFilter(typeof(CustomExceptionFilter))]
        [ProducesResponseType(typeof(string),(int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetailsModel), (int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> RegisterAccount(Account account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _accountService.RegisterAsync(account.Login, account.Password);
                
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                _logger.LogInformation($"Registered user:{result}");
                return Ok(result);
                
            }
            catch (Exception exception)
            {
                return BadRequest(new ErrorDetailsModel(19,exception.Message));
            }
        }
    }
}