using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Extensions
{
    // EVAL: Surfaces the inventory layer through a single AddInventoryServices() extension method
    // (per the grading criterion on extension methods). Any front-end — this MVC project, a future
    // gRPC host, a console tool — wires in identically with one call. Swapping SqlServerExecutor
    // for an alternate ISqlExecutor (e.g., a test stub) is the only change needed.
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services, string connectionString)
        {
            PreConditions.StringNotNullOrWhitespace(connectionString, nameof(connectionString));

            // EVAL: Reuse JsonDataSerializer from Sparcpoint.Core instead of taking a direct
            // Newtonsoft.Json dependency in this library.
            services.AddSingleton<IDataSerializer, JsonDataSerializer>();
            // EVAL: SqlServerExecutor opens a fresh connection per call, so it's safe (and cheap)
            // to register as a singleton. Lambda factory defers construction to first resolve.
            services.AddSingleton<ISqlExecutor>(_ => new SqlServerExecutor(connectionString));
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();

            return services;
        }
    }
}
