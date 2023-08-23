using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Application.Data;
using Sparcpoint.Inventory.Application.Repositories;
using Sparcpoint.Inventory.Application.Services;
using Sparcpoint.Inventory.Application.Services.Implementations;

namespace Sparcpoint.Inventory.Application
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services, string connectionString)
        {
            return services.AddInventorySqlContext(connectionString);
        }
        

        public static IServiceCollection AddInventorySqlContext(this IServiceCollection services, string connectionString)
        {
            return services.AddInventorySqlServices(connectionString)
                           .AddInventorySqlRepositories();
        }
        private static IServiceCollection AddInventorySqlRepositories(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IInstanceRepository<>), typeof(SqlInstanceRepository<>))
                           .AddScoped<IProductRepository, SqlProductRepository>()
                           .AddScoped<IProductAttributeRepository, SqlProductAttributeRepository>()
                           .AddScoped<IProductCategoryRepository, SqlProductCategoryRepository>()
                           .AddScoped<ICategoryRepository, SqlCategoryRepository>()
                           .AddScoped<ICategoryAttributeRepository, SqlCategoryAttributeRepository>()
                           .AddScoped<ICategoryCategoryRepository, SqlCategoryCategoryRepository>();
        }

        private static IServiceCollection AddInventorySqlServices(this IServiceCollection services, string connectionString)
        {
            return services.AddSqlExecutorFactory(connectionString)
                           .AddScoped<IProductService, SqlProductService>();
        }

        public static IServiceCollection AddInventoryMockContext(this IServiceCollection services)
        {
            return services.AddInventoryMockServices()
                           .AddInventoryMockRepositories();
        }

        private static IServiceCollection AddInventoryMockServices(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddInventoryMockRepositories(this IServiceCollection services)
        {
            return services;
        }
    }
}
