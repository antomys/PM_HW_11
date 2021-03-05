using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DepsWebApp
{
    /// <summary>
    /// Main program entry
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main method. Here begins program
        /// </summary>
        /// <param name="args">arguments as string array</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
