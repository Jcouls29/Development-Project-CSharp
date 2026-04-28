using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Data;
using Sparcpoint.SqlServer.Abstractions;

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
                                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection") ??
                                                       throw new InvalidOperationException("ConnectionString cannot be null");
                                services.AddSingleton(connectionString);
                                services.AddScoped<ProductsRepository>();
                                services.AddScoped<ProductController>();
                            });

            return builder;
        }
    }
}
