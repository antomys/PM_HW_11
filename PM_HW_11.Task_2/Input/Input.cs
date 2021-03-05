using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PM_HW_11.Task_2.Models;

namespace PM_HW_11.Task_2.Input
{
    public class Input
    {
        private static InputModel _model;
        private static readonly Random Random = new();
        public Input()
        {
            _model = InputModel.Construct();
        }
        
        public async Task TestRegistration(HttpClient httpClient)
        {
            //todo: make it 10 times
            var tasks = _model.Registration.Select(x => InternalTestRegistration(httpClient, x.Key, x.Value));
            await Task.WhenAll(tasks);
            

        }

        public async Task TestCurrencyConverter(HttpClient httpClient)
        {
            /*var tasks 
                = await Task.Factory.StartNew(() => GetPrimes.Select(pair => InternalTestCurrencyConverter(httpClient, pair.Key, pair.Value)));*/
            
            var tasks 
                = await Task.Factory.StartNew(() => 
                    _model.CurrencyChanger.Select(pair => InternalTestCurrencyConverter(httpClient, pair.Key, pair.Value)));
            await Task.WhenAll(tasks);

        }
        
        private static async Task InternalTestRegistration(HttpClient httpClient, string key, int value)
        {
            var inputUri = new Uri(httpClient.BaseAddress + key);
            try
            {
                var inputModel = new LoginModel()
                {
                    Login = RandomString(),
                    Password = RandomString()
                };
                var serialized = JsonSerializer.Serialize(inputModel);
                HttpContent content = 
                    new StringContent(serialized, Encoding.UTF8,"application/json");
                
                var responseMessage = await httpClient.PostAsync(inputUri,content);
                
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                
                var errorDeserialized = JsonSerializer.Deserialize<ErrorModel>(responseBody);
                
                if(errorDeserialized != null)
                    Console.WriteLine($"Input URL: [{inputUri}]\n" + 
                                      $"Expected Error code: [{value}]\n" +
                                      $"Received error code: [{errorDeserialized.Code}]\n" +
                                      $"Test passed: [{value == errorDeserialized.Code}]\n");
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
        private static async Task InternalTestCurrencyConverter(HttpClient httpClient, string key, 
            ConcurrentDictionary<string,HttpStatusCode> value)
        {
            try
            {
                var expectedCustomErrorCode  = value.Keys.First();
                value.TryGetValue(expectedCustomErrorCode, out var expectedStatusCode);
                
                var inputUri = httpClient.BaseAddress + key;
                
                var responseMessage = await httpClient.GetAsync(inputUri);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                if ((int)responseMessage.StatusCode != (int)HttpStatusCode.OK)
                {
                    Console.WriteLine($"Input URL: [{inputUri}]\nExpected Code:[{expectedStatusCode}]\n" +
                                      $"Received Code:[{responseMessage.StatusCode}]\n" +
                                      $"Test passed: [{responseMessage.StatusCode == expectedStatusCode}]\n");
                }
                else
                {
                    try
                    {
                        var errorDeserialized = JsonSerializer.Deserialize<ErrorModel>(responseBody);
                        Console.WriteLine($"Input URL: [{inputUri}]\nExpected Custom Error:[{expectedCustomErrorCode}]\n" +
                                          $"Received Code:[{errorDeserialized.Code}]\n" +
                                          $"Test passed: [{errorDeserialized.Code.ToString() == expectedCustomErrorCode}]\n");
                        //This is where i convert int code number to string to check custom error code with deserialized response custom error code
                        //because in connectionUrl this custom codes are strings
                    }
                    catch (Exception)
                    { 
                        var responseNumber = Convert.ToDecimal(responseBody); 
                        Console.WriteLine($"Input URL: [{inputUri}]\nExpected :[typeof(Decimal)]\n" +
                                              $"Received :[{responseNumber.GetType().Name}]\n" +
                                              $"Test passed: [{responseNumber.GetType().Name == "Decimal"}]\n");
                    }
                }
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
        private static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, Random.Next(6,10))
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}