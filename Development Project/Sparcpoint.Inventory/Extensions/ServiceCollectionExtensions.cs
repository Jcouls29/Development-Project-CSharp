using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.Inventory.Services;

namespace Sparcpoint.Inventory.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryService, InventoryService>();
            return services;
        }
    }
}
