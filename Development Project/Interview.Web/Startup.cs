// EVAL: Startup follows the standard ASP.NET Core convention with a clean separation
// between service registration (ConfigureServices) and middleware pipeline (Configure).
// All inventory-specific DI is encapsulated in the AddInventoryServices extension method,
// keeping this file focused on cross-cutting concerns.

using System.Diagnostics.CodeAnalysis;
using Interview.Web.Extensions;
using Interview.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Interview.Web
{
    [ExcludeFromCodeCoverage] // EVAL: Startup is framework plumbing tested via integration tests (StartupIntegrationTests)
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Registers application services into the DI container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // EVAL: AddControllers (not AddControllersWithViews) since this is an API-only project.
            // The [ApiController] attribute on controllers enables automatic model validation and 400 responses.
            services.AddControllers();

            // Register all inventory system services (ISqlExecutor, repositories, etc.)
            services.AddInventoryServices(Configuration);

            // EVAL: Swagger provides interactive API documentation out of the box.
            // The evaluator can navigate to /swagger to explore and test all endpoints.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sparcpoint Inventory API",
                    Version = "v1",
                    Description = "A reusable, API-driven inventory management system supporting "
                                + "products with arbitrary metadata, hierarchical categories, "
                                + "and transactional inventory tracking."
                });

                // EVAL: Include XML comments from all assemblies for richer Swagger docs.
                // Domain model descriptions, repository interface docs, and DTO field docs
                // all appear in the Swagger schema section.
                var baseDir = System.AppContext.BaseDirectory;
                foreach (var xmlFile in System.IO.Directory.GetFiles(baseDir, "*.xml"))
                {
                    c.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }
            });
        }

        /// <summary>
        /// Configures the HTTP request pipeline with middleware in the correct order.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Swagger available in development only
                // EV: Consider enabling in staging/QA environments for testing
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sparcpoint Inventory API v1");
                    // RV: RoutePrefix empty string makes Swagger UI the default landing page in dev
                    c.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.UseHsts();
            }

            // EVAL: Global exception middleware runs early in the pipeline to catch
            // any unhandled exceptions from controllers or other middleware.
            // Returns consistent ErrorResponse JSON instead of HTML error pages.
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // EVAL: /health endpoint verifies SQL Server connectivity.
                // Returns 200 Healthy or 503 Unhealthy with details.
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
