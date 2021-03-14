using Microsoft.AspNetCore.Authentication;

namespace DepsWebApp.Authentication
{
    /// <summary>
    /// Scheme options
    /// </summary>
    public class Base64SchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Base64SchemeOptions()
        {
            ClaimsIssuer = "PM_HW_12";
        }
        
    }
}