using System.Threading.Tasks;
using DepsWebApp.Models;

namespace DepsWebApp.Services
{
    /// <summary>
    /// Interface of account service
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Method to register a new account and return
        /// to him base64 output
        /// </summary>
        /// <param name="login">account login</param>
        /// <param name="password">Account password</param>
        /// <returns>encoded string</returns>
        Task<string> RegisterAsync(string login, string password);

        /// <summary>
        /// Method to get account by base64 string
        /// </summary>
        /// <param name="encodedString">encoded to base64 string</param>
        /// <returns>boolean</returns>
        Task<bool> GetAccount(string encodedString);
    }
}