using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PM_HW_11.Task_2.Input
{
    public class InputModel
    { 
        [JsonPropertyName("registration")]
        public static Dictionary<string,HttpStatusCode> Registration { get; set; }
        
        [JsonPropertyName("currencychanger")]
        public static Dictionary<List<string>,decimal> CurrencyChanger { get; set; }
        
        public static InputModel Construct()
        {
            var deserialized = JsonSerializer.Deserialize<InputModel>(File.ReadAllText("connectionUrl.json"));

            return deserialized;
        }
    }
}