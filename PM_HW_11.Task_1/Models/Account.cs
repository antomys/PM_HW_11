using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DepsWebApp.Models
{
    /// <summary>
    /// Registration mode
    /// </summary>
    [Table("Accounts")]
    public class Account
    {
        /// <summary>
        /// User login property
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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