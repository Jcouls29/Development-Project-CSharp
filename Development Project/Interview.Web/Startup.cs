using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;

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

            // EVAL: SqlServerExecutor is stateless (connection string only), so singleton lifetime
            // avoids repeatedly resolving the options on every request.
            string connectionString = Configuration.GetConnectionString("InventoryDb");
            services.AddSingleton<ISqlExecutor>(new SqlServerExecutor(connectionString));

            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();   // serves index.html at /
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
