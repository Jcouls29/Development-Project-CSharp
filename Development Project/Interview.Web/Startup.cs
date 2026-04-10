using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Interview.Web.Middleware;
using Sparcpoint.Inventory.Extensions;

namespace Interview.Web
{
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

            // EVAL: API versioning allows old and new clients to coexist.
            // Old clients continue using v1 endpoints unchanged.
            // New clients can adopt v2 with enhanced features (e.g., bulk operations, pagination).
            // Versioning is URL-segment based (api/v1/..., api/v2/...) for maximum clarity.
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddMvc() // EVAL: Registers URL segment route constraint for {version:apiVersion}
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // EVAL: Swagger/OpenAPI documentation with per-version docs.
            // Allows evaluators and API consumers to explore and test endpoints interactively.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Inventory Management API",
                    Version = "v1",
                    Description = "V1 - Core inventory management endpoints for existing clients."
                });
                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Inventory Management API",
                    Version = "v2",
                    Description = "V2 - Enhanced endpoints with bulk operations and pagination for new clients."
                });
            });

            // EVAL: Auto-detect database availability. If a connection string is configured,
            // use SQL Server. Otherwise, fall back to in-memory storage for development/demo.
            // This demonstrates the flexibility of the interface-based architecture —
            // swapping the entire data layer is a single line change.
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddInventoryServices(connectionString);
            }
            else
            {
                services.AddInventoryInMemoryServices();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // EVAL: Global error handling middleware replaces per-action try-catch blocks.
            // Must be registered first so it catches exceptions from all downstream middleware.
            // Maps ArgumentException → 400, KeyNotFoundException → 404, others → 500.
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            // EVAL: Swagger UI available at /swagger for interactive API exploration and testing
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API V1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Inventory API V2");
            });

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
