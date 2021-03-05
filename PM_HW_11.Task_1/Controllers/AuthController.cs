using System;
using System.Net;
using System.Threading.Tasks;
using DepsWebApp.Middlewares;
using DepsWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Authenticator controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AuthController()
        {
            
        }
        
        /// <summary>
        /// Method to register user account
        /// </summary>
        /// <param name="registration">JSON of user data</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">this method is not implemented</exception>
        [HttpPost]
        [TypeFilter(typeof(CustomExceptionFilter))]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetailsModel),(int) HttpStatusCode.BadRequest)]
        public async Task<bool> RegisterAccount(Registration registration)
        {
            throw new NotImplementedException();
        }
    }
}