using System.Text.Json.Serialization;

namespace DepsWebApp.Models
{
    /// <summary>
    /// Registration mode
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// User login property
        /// </summary>
        [JsonPropertyName("login")]
        public string Login { get; set; }
        
        /// <summary>
        /// User password property
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}