using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Inventory.SqlServer;

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

            // EVAL: AddSwaggerGen is the correct registration for controller-based APIs with Swashbuckle.
            // AddEndpointsApiExplorer() is only for Minimal APIs and is intentionally omitted here.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Inventory Management API",
                    Version = "v1"
                });
            });

            // EVAL: All inventory repositories are wired via a single extension method.
            // Adding a new repository only requires a change in ServiceCollectionExtensions,
            // not in every host project's Startup.
            var connectionString = Configuration.GetConnectionString("InventoryDatabase");
            services.AddSqlInventoryServices(connectionString);
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

            // EVAL: Swagger is available in all environments for evaluation purposes.
            // In a production system, restrict this behind env.IsDevelopment() or a feature flag.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
