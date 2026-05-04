using Interview.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sparcpoint.API.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Interview.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseStartup<Startup>();
                            })
                            .ConfigureServices((hostContext, services) =>
                            {
                                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnectionString") ??
                                                       throw new InvalidOperationException("ConnectionString cannot be null");
                                services.AddSingleton(connectionString);
                                services.AddScoped<ProductController>();
                                services.AddScoped<Products>();
                            });

            return builder;
        }
    }
}

