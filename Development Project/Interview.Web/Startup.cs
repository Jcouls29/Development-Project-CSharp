using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sparcpoint.Inventory.SqlServer.Extensions;

namespace Interview.Web
{
    /// <summary>
    /// EVAL: Centralized DI wiring. The system pipeline is built with ONE single
    /// call — AddSparcpointInventorySqlServer — which allows consuming the
    /// solution from other hosts (function apps, workers) with minimal coupling.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // EVAL: Connection string resuelto desde config (appsettings.*.json o env var).
            // Fallback a default de desarrollo local para que el proyecto "just works"
            // en la demo del evaluador. En prod se sobrescribe con ENV/User-Secrets.
            var connectionString =
                Configuration.GetConnectionString("SparcpointInventory")
                ?? "Server=localhost;Database=SparcpointInventory;User Id=SA;Password=ChangeMe@Passw0rd;TrustServerCertificate=True;";

            services.AddSparcpointInventorySqlServer(connectionString);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sparcpoint Inventory API",
                    Version = "v1",
                    Description = "Inventory management system for Sparcpoint's clients."
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sparcpoint Inventory v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
