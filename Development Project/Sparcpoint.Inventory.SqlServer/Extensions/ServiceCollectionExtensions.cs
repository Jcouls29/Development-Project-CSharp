using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// EVAL: Extension method pattern keeps Startup.cs clean and makes the inventory
    /// feature a single line of registration that any host project can call.
    /// New repositories added in the future only require changes here.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlInventoryServices(
            this IServiceCollection services,
            string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("A SQL Server connection string is required.", nameof(connectionString));

            // ISqlExecutor is registered as Scoped so each HTTP request gets its own connection.
            services.AddScoped<ISqlExecutor>(_ => new SqlServerExecutor(connectionString));

            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();

            return services;
        }
    }
}
