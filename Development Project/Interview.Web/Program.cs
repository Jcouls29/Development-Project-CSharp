using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger =  Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            logger.Information($"Starting Interview.Web ..");

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
			.ConfigureLogging(logger =>
			{
                logger.ClearProviders();
                logger.AddSerilog();
			})
			.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName.ToLowerInvariant()}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
