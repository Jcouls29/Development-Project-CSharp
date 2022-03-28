using Interview.Web.Interfaces;
using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;

namespace Interview.Web
{
    public class Program
    {

        //EVAL: Application middleware is all configured here in Program.cs eliminating Startup.cs
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        public static void Main(string[] args)
        {
            //EVAL: Added Serilog.AspNetCore 'nuget' package
            Serilog.Events.LogEventLevel logEventLevel = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel),
                Configuration.GetValue<string>("Log:MinimumLevel"), true);

            //EVAL: 25 mb is file limit size for one file
            int logFileSizeLimitBytes = Convert.ToInt32(Configuration.GetValue<string>("Log:FileLimitSizeBytes"));
            //EVAL: Maximum number of files to store, files will be automatically deleted once this limit is reached
            int logFilesRetainedFileCount = Convert.ToInt32(Configuration.GetValue<string>("Log:FileRetainCount"));
            //EVAL:Configuring serilog to write logs asynchronously to improve performance. For this Serilog.Sinks.Async 'nuget' package 
            Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", logEventLevel).Enrich
                .FromLogContext().WriteTo.Async(writeTo => writeTo.File(Configuration.GetValue<string>("Log:Path"),
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: logFileSizeLimitBytes,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: logFilesRetainedFileCount))
                .CreateLogger();
            try
            {
                Log.Information("Starting Product API");
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                builder.Services.AddControllers();
                //EVAL:adding sql db context and string
                //EVAL: Added Microsoft.EntityFrameworkCore.SqlServer 'nuget' package
                builder.Services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SQLServerConnString")));
                builder.Services.AddScoped<IProductRepo, ProductService>();
                //EVAL: Adding swagger specs that has api documentation and functionality. Added Swashbuckle.AspNetCore 'nuget' package for this
                builder.Services.AddSwaggerGen(s =>
                {
                    s.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Products Assignment",
                        Version = "v1"
                    });
                });
                var app = builder.Build();
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                //EVAL: disabling swagger in Production
                if (!app.Environment.IsProduction())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(s =>
                    {
                        s.SwaggerEndpoint("v1/swagger.json", "Products Assignment Api V1");
                    });
                }
                app.UseRouting();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Service terminated unexpectedly with this exeption: " + ex.Message.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
