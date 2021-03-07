using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Required!")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum 6 characters required")]
        [StringLength(24,MinimumLength = 6,ErrorMessage = "Maximum 24 characters")]
        [JsonPropertyName("login")]
        public string Login { get; set; }
        
        /// <summary>
        /// User password property
        /// </summary>
        [Required(ErrorMessage = "Required!")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Minimum 6 characters required")]
        [StringLength(24,MinimumLength = 6,ErrorMessage = "Maximum 24 characters")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}