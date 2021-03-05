using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        
        public static async Task TestRegistration(HttpClient httpClient)
        {
            var listOfTasks = new List<Task>();

            var thisString = _model.Registration.Keys.First();
             _model.Registration.TryGetValue(thisString, out var thisCode);
             
             for (var i = 0; i < 10; i++)
            {
                listOfTasks
                    .Add(InternalTestRegistration(httpClient,thisString,thisCode));
            }
            await Task.WhenAll(listOfTasks);
            

        }

        public static async Task TestCurrencyConverter(HttpClient httpClient)
        {
            
            var tasks 
                = await Task.Factory.StartNew(() => 
                    _model.CurrencyChanger
                        .Select(pair => 
                            InternalTestCurrencyConverter(httpClient, pair.Key, pair.Value)));
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
                                      $"Input Body: {inputModel}\n" + 
                                      $"Expected Error code: [{value}]\n" +
                                      $"Received error code: [{errorDeserialized.Code}]\n" +
                                      $"Received Body: {errorDeserialized}" +
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
                                      $"Received body {responseBody}\n" +
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
                                          $"Received Body: {errorDeserialized}\n" +
                                          $"Test passed: [{errorDeserialized.Code.ToString() == expectedCustomErrorCode}]\n");
                        //This is where i convert int code number to string to check custom error code with deserialized response custom error code
                        //because in connectionUrl this custom codes are strings
                    }
                    catch (Exception)
                    { 
                        var responseNumber = Convert.ToDecimal(responseBody); 
                        Console.WriteLine($"Input URL: [{inputUri}]\nExpected :[typeof(Decimal)]\n" +
                                              $"Received :[{responseNumber.GetType().Name}]\n" +
                                              $"Received Body:{responseNumber}\n" +
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