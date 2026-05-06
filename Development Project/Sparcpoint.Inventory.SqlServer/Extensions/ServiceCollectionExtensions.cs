using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// EVAL: keeps Startup.cs clean - the whole inventory feature is one line from any host project.
    /// New repositories just get added here, nothing else to touch.
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
