using System.Threading.Tasks;
using DepsWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DepsWebApp.Middlewares
{
    /// <summary>
    /// Filter to catch exceptions and generate readable output
    /// </summary>
    public class CustomExceptionFilter : IAsyncExceptionFilter
    {
        /// <inheritdoc />
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionName = context.Exception.GetType().Name;
            var errorCode = ConstructErrorResponse(exceptionName.ToLower());

            var error = new ErrorDetailsModel
            {
                ErrorCode = errorCode,
                ErrorMessage = context.Exception.Message
            };

            context.Result = new JsonResult(error);
            return Task.CompletedTask;
        }
        
        private static int? ConstructErrorResponse(string exceptionName)
        {
            if (exceptionName.Contains("null"))
                return 11;
            if (exceptionName.Contains("implemented"))
                return 13;
            if (exceptionName.Contains("signed"))
                return 15;
            return exceptionName.Contains("invalid") ? 17 : 19;
        }
        
    }
}