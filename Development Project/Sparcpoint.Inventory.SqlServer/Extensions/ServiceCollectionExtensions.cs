using System;
using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.Inventory.SqlServer.Repositories;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.SqlServer.Extensions
{
    /// <summary>
    /// EVAL: Extension methods to wire up the system in any host (Interview.Web,
    /// functions, etc). Keeps "a single place" to register the whole stack.
    /// Follows the conventional AspNetCore pattern (AddX/AddXyz).
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the ISqlExecutor and the inventory system repositories.
        /// </summary>
        /// <param name="services">Service collection that the services will be added to.</param>
        /// <param name="connectionString">SQL Server connection string.</param>
        public static IServiceCollection AddSparcpointInventorySqlServer(
            this IServiceCollection services, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            // EVAL: Scoped so each request has its own connection/transaction.
            // If a route needs to share a transaction across multiple repos, a
            // TransactionSqlServerExecutor can be used from a Unit of Work.
            services.AddScoped<ISqlExecutor>(_ => new SqlServerExecutor(connectionString));
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();

            return services;
        }
    }
}
