using System.Text.Json.Serialization;

namespace PM_HW_11.Task_2.Models
{
    public class ErrorModel
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; }
        
        [JsonPropertyName("message")]
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return $"Code: {Code}; Message: {ErrorMessage}";
        }
    }
}