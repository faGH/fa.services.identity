using FrostAura.Services.Identity.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FrostAura.Services.Identity.Api
{
    /// <summary>
    /// Entry class to application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry method to the application.
        /// </summary>
        /// <param name="args">Console arguments.</param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            host.Run();
        }

        /// <summary>
        /// Build host container.
        /// </summary>
        /// <param name="args">Console arguments.</param>
        /// <returns>Host conatiner.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
