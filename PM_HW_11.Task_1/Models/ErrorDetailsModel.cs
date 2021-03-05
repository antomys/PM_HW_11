using System.Text.Json.Serialization;

namespace DepsWebApp.Models
{
    /// <summary>
    /// Model of error details
    /// </summary>
    public class ErrorDetailsModel
    {
        /// <summary>
        /// Code or error, nullable
        /// </summary>
        [JsonPropertyName("code")]
        public int? ErrorCode { get; set; }
        
        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; }
        
    }
}