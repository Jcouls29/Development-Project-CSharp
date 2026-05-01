using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory
{
    public static class ServiceCollectionExtensions
    {
        // EVAL: Extension method groups all inventory registrations in one call, keeping Startup.cs clean
        // and making it easy to wire up the same services in a different web project
        public static IServiceCollection AddInventoryServices(this IServiceCollection services, string connectionString)
        {
            PreConditions.StringNotNullOrWhitespace(connectionString, nameof(connectionString));

            // EVAL: Scoped rather than Singleton for two reasons:
            // 1. Captive dependency safety — a Singleton holds any injected Scoped dependency
            //    forever (e.g. IHttpContextAccessor), causing stale state across requests.
            //    Scoped ensures a fresh instance per request if dependencies are added later.
            // 2. Multi-tenancy readiness — the factory lambda runs per request, so swapping to
            //    a per-tenant connection string later requires one change here, nothing downstream:
            //    provider => new SqlServerExecutor(provider.GetRequiredService<ITenantContext>().ConnectionString)
            // Factory lambda is used because SqlServerExecutor takes a plain string, not IOptions<T>.
            services.AddScoped<ISqlExecutor>(_ => new SqlServerExecutor(connectionString));
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();

            return services;
        }
    }
}
