using System.Text.Json.Serialization;

namespace PM_HW_11.Task_2.Models
{
    public class LoginModel
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }

        public LoginModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public override string ToString()
        {
            return $"Login:{Login}; Password: {Password}";
        }
    }
}