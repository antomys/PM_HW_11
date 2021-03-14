using System.Collections.Generic;
using System.Security.Claims;

namespace DepsWebApp.Authentication
{
    /// <summary>
    /// Account identity
    /// </summary>
    public class AccountIdentity : ClaimsIdentity
    {
        /// <summary>
        /// base64
        /// </summary>
        public string Base64String { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountIdentity()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="base64String">encoded string</param>
        public AccountIdentity(string base64String)
        :base(CreateClaimsIdentity(base64String),Base64Scheme.Type)
        {
            Base64String = base64String;
        }
        private static IEnumerable<Claim> CreateClaimsIdentity(
            string base64Encoded)
        {
            var result = new List<Claim>();
            if(base64Encoded != null)
                result.Add(new Claim(DefaultNameClaimType, base64Encoded));
            return result;
        }
    }
}