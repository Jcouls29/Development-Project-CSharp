using Interview.Web.Data;
using Interview.Web.Infrastructure;
using Interview.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.SqlServer.Abstractions;
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
            // EVAL: Reusing the provided SQL executor keeps the solution close to the starter project
            // and avoids adding a new ORM or persistence stack during an interview-sized exercise.
            services.AddSingleton<ISqlExecutor>(_ =>
            {
                var connectionString = Configuration.GetConnectionString("Inventory");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("ConnectionStrings:Inventory must be configured before the API can start.");

                return new SqlServerExecutor(connectionString);
            });

            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
            services.AddScoped<IInventoryManagementService, InventoryManagementService>();
            services.AddScoped<ApiExceptionFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error");

            if (!env.IsDevelopment())
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
