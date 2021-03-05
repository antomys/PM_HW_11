using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PM_HW_11.Task_2
{
    
    internal static class Program
    {
        private static readonly HttpClient Client = new();

        private static async Task Main()
        {
            Client.BaseAddress = new Uri("http://localhost:5000");
            var input = new Input.Input();
            
            Console.WriteLine("*********************************************");
            await Input.Input.TestRegistration(Client);
            Console.WriteLine("*********************************************");
            await Input.Input.TestCurrencyConverter(Client);
        }
        
    }
}