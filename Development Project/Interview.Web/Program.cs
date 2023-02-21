using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Interview.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //EVAL: Configuring logging
            //var logger = NLogBuilder.ConfigureNLog($"{AppContext.BaseDirectory}/config/nlog/nlog.config").GetCurrentClassLogger();
            //var logger = NLog.LogManager.Setup().LoadConfigurationFromFile($"{ AppContext.BaseDirectory}/config/nlog/nlog.config").GetCurrentClassLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //logger.Error(exception);
                throw;
            }
            finally
            {
                //LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                    .UseLamar(); //EVAL: Configuring IoC Lamar
    }
}
