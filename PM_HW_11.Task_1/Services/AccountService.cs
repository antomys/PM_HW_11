using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DepsWebApp.Services
{
    internal class AccountService: IAccountService
    {
        private readonly BlockingCollection<string> _accounts = new BlockingCollection<string>();
        
        ///<inheritdoc/>
        public Task<string> RegisterAsync(string login, string password)
        {
            //This could be removed as field attributes in model are capable of doing that.
            if (string.IsNullOrEmpty(login))
                throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            
            var toEncodeString = $"{login}:{password}";
            var encodedCredentials = Base64Encode(toEncodeString);

            if (_accounts.Contains(encodedCredentials))
            {
                return Task.FromException<string>(new InvalidOperationException("Account already exists"));
            }

            _accounts.TryAdd(encodedCredentials);
            return Task.FromResult(encodedCredentials);
            
        }

        /// <inheritdoc />
        public Task<bool> GetAccount(string encodedString)
        {
            return Task.FromResult(_accounts.Contains(encodedString));
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
    }
}