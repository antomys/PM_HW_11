using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PM_HW_11.Task_2.Input
{
    public class InputModel
    { 
        [JsonPropertyName("registration")]
        public ConcurrentDictionary<string,int> Registration { get; set; }
        //where string = http request string and int is error code value
        
        [JsonPropertyName("currencychanger")]
        public ConcurrentDictionary<string,ConcurrentDictionary<string,HttpStatusCode>> CurrencyChanger { get; set; }
        
        public static InputModel Construct()
        {
            var deserialized = JsonSerializer.Deserialize<InputModel>(File.ReadAllText("connectionUrl.json"));

            return deserialized;
        }
    }
}