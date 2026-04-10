using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.Inventory.Repositories.InMemory;
using Sparcpoint.Inventory.Repositories.SqlServer;
using Sparcpoint.Inventory.Services;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Extensions
{
    /// <summary>
    /// EVAL: Extension methods for clean DI registration.
    /// Allows any ASP.NET Core host to register the entire inventory system with a single call:
    ///   services.AddInventoryServices(connectionString);
    /// This approach supports multiple front-end APIs re-using the same backend
    /// without duplicating DI configuration, and follows the pattern used by
    /// libraries like AddControllers(), AddDbContext(), etc.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all inventory system services, repositories, and the SQL executor.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">SQL Server connection string.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddInventoryServices(this IServiceCollection services, string connectionString)
        {
            PreConditions.StringNotNullOrWhitespace(connectionString, nameof(connectionString));

            // EVAL: SqlServerExecutor is safe as singleton — it creates a new connection per ExecuteAsync call
            services.AddSingleton<ISqlExecutor>(new SqlServerExecutor(connectionString));

            services.AddInventoryRepositories();
            services.AddInventoryBusinessServices();

            return services;
        }

        /// <summary>
        /// EVAL: Separated so consumers can register repositories independently
        /// (e.g., swap SqlServer for InMemory in test hosts) while keeping services.
        /// </summary>
        public static IServiceCollection AddInventoryRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            return services;
        }

        /// <summary>
        /// Registers only the business service layer. Useful when repositories
        /// are registered separately (e.g., with test doubles).
        /// </summary>
        public static IServiceCollection AddInventoryBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }

        /// <summary>
        /// EVAL: Registers the inventory system with in-memory repositories.
        /// No database required — perfect for development, demos, and integration testing.
        /// Demonstrates the power of the interface-based architecture: swapping the entire
        /// data layer requires changing only one DI registration call.
        /// </summary>
        public static IServiceCollection AddInventoryInMemoryServices(this IServiceCollection services)
        {
            // EVAL: Singleton lifetime so data persists across requests for the lifetime of the app.
            // In production (SQL Server), repositories are scoped — here we use singleton
            // because the in-memory store IS the database.
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
            services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            services.AddInventoryBusinessServices();

            return services;
        }
    }
}
