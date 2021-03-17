using System;
using System.Linq;
using System.Threading.Tasks;
using DepsWebApp.Models;

namespace DepsWebApp.Services
{
    internal class AccountService: IAccountService
    {
        //private readonly ConcurrentDictionary<string, Account> _accounts = new ConcurrentDictionary<string, Account>();
        private readonly ApplicationDbContext _applicationDbContext;

        public AccountService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        
        ///<inheritdoc/>
        public Task<string> RegisterAsync(string login, string password)
        {
            //This could be removed as field attributes in model are capable of doing that.
            if (string.IsNullOrEmpty(login))
                throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            
            if (_applicationDbContext.Accounts.Find(login) != null)
            {
                return Task.FromException<string>(new InvalidOperationException("Account already exists"));
            }

            _applicationDbContext.Accounts.AddAsync(new Account {Login = login, Password = password});
            _applicationDbContext.SaveChangesAsync();
            
            return Task.FromResult(string.Empty);
        }

        ///<inheritdoc/>
        public Task<bool> GetAccount(string encodedString)
        {
            var credentials = Base64Decode(encodedString).Split(":");
            return Task.FromResult(_applicationDbContext.Accounts.Any(x=> x.Login==credentials[0] && x.Password==credentials[1]));
        }
        
        /// <summary>
        /// Method to encode account credentials to Base64
        /// </summary>
        /// <param name="plainText">input string</param>
        /// <returns>encoded to base 64 string</returns>
        private static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        /// <summary>
        /// Method to decode account credentials to Base64
        /// </summary>
        /// <param name="base64EncodedData">input encoded string</param>
        /// <returns>decoded string</returns>
        private static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}