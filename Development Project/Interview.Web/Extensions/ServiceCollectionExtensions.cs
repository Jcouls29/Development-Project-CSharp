// EVAL: Extension methods provide a clean, single-line DI registration pattern.
// This allows any new front-end API project to onboard the inventory system
// with just one call: services.AddInventoryServices(configuration).
// This approach follows the pattern used by Microsoft's own libraries (e.g., AddControllers, AddSwaggerGen).

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.SqlServer;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Extensions
{
    /// <summary>
    /// Extension methods for configuring inventory system services in the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all inventory system services including data access and configuration.
        /// </summary>
        /// <param name="services">The service collection to register services into.</param>
        /// <param name="configuration">Application configuration for reading connection strings.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddInventoryServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // EVAL: Bind SqlServerOptions from configuration so the connection string
            // is configurable per environment without code changes.
            services.Configure<SqlServerOptions>(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("SparcpointInventory");
            });

            // Register ISqlExecutor as scoped so each request gets its own connection lifecycle.
            services.AddScoped<ISqlExecutor>(sp =>
            {
                var connectionString = configuration.GetConnectionString("SparcpointInventory");
                return new SqlServerExecutor(connectionString);
            });

            // EVAL: Health check verifies SQL Server connectivity at /health.
            // Useful for Docker orchestration, load balancers, and monitoring.
            var connectionString = configuration.GetConnectionString("SparcpointInventory");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddHealthChecks()
                    .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "db", "ready" });
            }

            // EVAL: Repositories registered as scoped -- one instance per HTTP request.
            // Interface -> Implementation pattern enables unit testing with mocks
            // and swapping data stores without touching controllers.
            services.AddScoped<IProductRepository, SqlServerProductRepository>();
            services.AddScoped<IInventoryRepository, SqlServerInventoryRepository>();

            return services;
        }
    }
}
