using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
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
        private static readonly ConcurrentQueue<string> _base64Collection = new();
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
                    .Add(InternalTestRegistration(httpClient,thisString,thisCode,true));
                listOfTasks
                    .Add(InternalTestRegistration(httpClient,thisString,thisCode,false));
            }
            await Task.WhenAll(listOfTasks);
            

        }

        public async Task TestCurrencyConverter(HttpClient httpClient)
        {
            
            var tasks 
                = await Task.Factory.StartNew(() => 
                    _model.CurrencyChanger
                        .Select(pair => 
                            InternalTestCurrencyConverter
                                (httpClient, pair.Key, pair.Value)));
            await Task.WhenAll(tasks);

        }
        
        private static async Task InternalTestRegistration(HttpClient httpClient, string key, int value, bool isValid)
        {
            var inputUri = new Uri(httpClient.BaseAddress + key);
            
            try
            {
                LoginModel inputModel;
                var expectedEncoded = "";
                
                if (!isValid)
                    inputModel = new LoginModel(RandomString(1, 5), RandomString(25, 28));
                else
                {
                    inputModel = new LoginModel(RandomString(6, 24), RandomString(6, 24));
                    expectedEncoded = Base64Encode($"{inputModel.Login}:{inputModel.Password}");
                }
               
                
                var serialized = JsonSerializer.Serialize(inputModel);
                HttpContent content = 
                    new StringContent(serialized, Encoding.UTF8,"application/json");
                
                var responseMessage = await httpClient.PostAsync(inputUri,content);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                if (responseBody != null && isValid)
                {
                    if(!_base64Collection.Contains(responseBody))
                        _base64Collection.Enqueue(responseBody);
                    Console.WriteLine($"Input URL: [{inputUri}]\n" +
                                      $"Input Body: {inputModel}\n" + 
                                      $"Expected code: [{value}]\n" +
                                      $"Expected Encoded: [{expectedEncoded}]\n" +
                                      $"Expected Decoded: [{Base64Decode(expectedEncoded)}]\n" +
                                      $"Received code: [{responseMessage.StatusCode}]\n" +
                                      $"Received Encoded: [{responseBody}]\n" +
                                      $"Received Decoded: [{Base64Decode(responseBody)}]\n" +
                                      $"Test passed: [{expectedEncoded==responseBody}]\n");
                }
                else
                {
                    Console.WriteLine($"Input URL: [{inputUri}]\n" +
                                      $"Input Body: {inputModel}\n" + 
                                      $"Expected code: [{HttpStatusCode.BadRequest}]\n" +
                                      $"Received code: [{responseMessage.StatusCode}]\n" +
                                      $"Received Error message: [{responseBody}]\n" +
                                      $"Test passed: [{responseMessage.StatusCode == HttpStatusCode.BadRequest}]\n");
                }
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
        
        private static async Task InternalTestCurrencyConverter(HttpClient httpClient, 
            string key, 
            ConcurrentDictionary<string,HttpStatusCode> value)
        {
            var token = "";
            if (_base64Collection.Count !=0) _base64Collection.TryDequeue(out token);
                
            try
            {
                var expectedCustomErrorCode  = value.Keys.First();
                value.TryGetValue(expectedCustomErrorCode, out var expectedStatusCode);
                
                var inputUri = httpClient.BaseAddress + key;

                httpClient.DefaultRequestHeaders.Authorization 
                    = new AuthenticationHeaderValue("base64", token);
                
                var responseMessage = await httpClient.GetAsync(inputUri);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                if ((int)responseMessage.StatusCode == (int)HttpStatusCode.Unauthorized)
                    Output(inputUri,(int)HttpStatusCode.Unauthorized,responseMessage);
                else if ((int) responseMessage.StatusCode != (int) HttpStatusCode.OK)
                    Output(inputUri,(int)expectedStatusCode,responseMessage,responseBody);
                else
                {
                    try
                    {
                        var errorDeserialized = JsonSerializer.Deserialize<ErrorModel>(responseBody);
                        Console.WriteLine($"Input URL: [{inputUri}]\n" +
                                          $"Authorization header: {token}\n" +
                                          $"Expected Custom Error:[{expectedCustomErrorCode}]\n" +
                                          $"Received Code:[{errorDeserialized.Code}]\n" +
                                          $"Received Body: {errorDeserialized}\n" +
                                          $"Test passed: [{errorDeserialized.Code.ToString() == expectedCustomErrorCode}]\n");
                        //This is where i convert int code number to string to check custom error code with deserialized response custom error code
                        //because in connectionUrl this custom codes are strings
                    }
                    catch (Exception)
                    { 
                        var responseNumber = Convert.ToDecimal(responseBody); 
                        Console.WriteLine($"Input URL: [{inputUri}]\n" +
                                          $"Authorization header: {token}\n" +
                                          $"Expected :[{typeof(decimal)}]\n" +
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
        private static string RandomString(int from, int to)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, Random.Next(from,to))
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
        private static string Base64Encode(string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static void Output(string inputUri, int expectedCode, 
            HttpResponseMessage responseBody, string responseMessage = "")
        {
            Console.WriteLine($"Input URL: [{inputUri}]\n" +
                              $"Expected Code:[{expectedCode}]\n" +
                              $"Received body {responseMessage}\n" +
                              $"Received Code:[{responseBody.StatusCode}]\n" +
                              $"Test passed: [{responseBody.StatusCode == (HttpStatusCode) expectedCode}]\n");
        }
        
        private static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}