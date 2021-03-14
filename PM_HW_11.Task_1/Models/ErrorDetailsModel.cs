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

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="errorMessage">Error message</param>
        public ErrorDetailsModel(int? errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}