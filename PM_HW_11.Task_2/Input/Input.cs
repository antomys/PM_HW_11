using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PM_HW_11.Task_2.Input
{
    public class Input
    {
        public Input()
        {
            InputModel.Construct();
        }

        public static async Task TestLandingPage(HttpClient httpClient)
        {
            var result = false;
            try
            {
                var responseMessage
                    = await httpClient.GetAsync(httpClient.BaseAddress);
                responseMessage.EnsureSuccessStatusCode();
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                const string expectedOutput = "Ihor Volokhovych, PM_HW_10, PM_HW_10.Task_1";

                if (responseBody.Equals(expectedOutput))
                {
                    result = true;
                }
                Console.WriteLine($"Input URL: [{LandingTest}]\n" +
                                  $"Expected: [{expectedOutput}]\n" +
                                  $"Received: [{responseBody}]\n" +
                                  $"Test passed: [{result.ToString()}]\n");
                
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        public static async Task TestIsPrime(HttpClient httpClient)
        {
            var tasks = IsPrime
                .Select(pair => InternalTestIsPrime(httpClient, pair.Key, pair.Value));
            await Task.WhenAll(tasks);
            

        }

        public async Task TestGetPrimes(HttpClient httpClient)
        {
            var tasks 
                = await Task.Factory.StartNew(() => GetPrimes.Select(pair => InternalTestGetPrimes(httpClient, pair.Key, pair.Value)));
            await Task.WhenAll(tasks);

        }
        
        private static async Task InternalTestIsPrime(HttpClient httpClient, string key, HttpStatusCode value)
        {
            var inputUri = new Uri(httpClient.BaseAddress + key);
            try
            {
                var responseMessage = await httpClient.GetAsync(inputUri);
                
                Console.WriteLine($"Input URL: [{inputUri}]\n" +
                                  $"Expected: [{value}]\n" +
                                  $"Received: [{responseMessage.StatusCode}]\n" +
                                  $"Test passed: [{value == responseMessage.StatusCode}]\n");
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        private static async Task InternalTestGetPrimes(HttpClient httpClient, string key, IReadOnlyCollection<int> value)
        {
            try
            {
                var inputUri = httpClient.BaseAddress + key;
                var responseMessage = await httpClient.GetAsync(inputUri);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                responseBody = Regex.Replace(responseBody, @"[\[\]]", "");  
               
                if (responseMessage.StatusCode.Equals(HttpStatusCode.OK))
                {
                    var numbers = new List<int>();
                    
                    if (!string.IsNullOrEmpty(responseBody) && responseBody.Any(char.IsDigit))
                    {
                        numbers =  responseBody.Split(',').Select(int.Parse).ToList();
                    }
                    
                    if (value.All(numbers.Contains) && value.Count == numbers.Count)
                    {
                        Console.WriteLine($"Input URL: [{inputUri}]\nExpected: [{string.Join(",", value)}]\n" +
                                          $"Received: [{responseBody}]\n" +
                                          $"Test passed: [{true}]\n");
                    }
                }
                else
                {
                    Console.WriteLine($"Input URL: [{inputUri}]\nExpected Code:[{HttpStatusCode.BadRequest}]\n" +
                                      $"Received Code:[{responseMessage.StatusCode}]\n" +
                                      $"Test passed: [{responseMessage.StatusCode == HttpStatusCode.BadRequest}]\n");
                }
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    }
}