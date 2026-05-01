using Interview.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Inventory;
using System;

namespace Interview.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddProblemDetails();
            services.AddExceptionHandler<InventoryExceptionHandler>();
            // EVAL: Environment variable takes precedence so local/CI connection strings
            // never need to be committed to source control. Falls back to appsettings.json.
            var connectionString = Environment.GetEnvironmentVariable("INVENTORY_DB")
                ?? Configuration.GetConnectionString("InventoryDb");
            services.AddInventoryServices(connectionString);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // EVAL: UseExceptionHandler() with no path uses the registered IExceptionHandler
            // (InventoryExceptionHandler) in all environments — consistent structured error
            // responses in dev and prod, no stack trace leakage.
            app.UseExceptionHandler();

            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
