using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Repositories.InMemory;
using Sparcpoint.Inventory.Repositories.Sql;
using Sparcpoint.Inventory.Services;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Sparcpoint.Inventory.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// EVAL: Single registration entry point — consumers call AddInventory
        /// and get the whole stack; repository choice driven by options so
        /// integration tests can swap in in-memory without duplicating wiring.
        public static IServiceCollection AddInventory(this IServiceCollection services, Action<InventoryOptions> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));

            var options = new InventoryOptions();
            configure(options);

            if (options.UseInMemoryRepositories)
            {
                services.AddSingleton<IProductRepository, InMemoryProductRepository>();
                services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
                services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.ConnectionString))
                    throw new InvalidOperationException("InventoryOptions.ConnectionString is required unless UseInMemoryRepositories is true.");

                services.AddSingleton<ISqlExecutor>(_ => new SqlServerExecutor(options.ConnectionString!));
                services.AddScoped<IProductRepository, SqlProductRepository>();
                services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
                services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            }

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}
