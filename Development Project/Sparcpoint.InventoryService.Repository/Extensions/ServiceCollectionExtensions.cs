using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Sparcpoint.Inventory.Repository.Interfaces;
using Sparcpoint.Inventory.Repository.Repositories;
using Sparcpoint.Inventory.Repository.Services;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.InventoryService.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration, /*IOptionsMonitor<ApplicationConfig> applicationSettings,*/ ILogger logger)
        {
            //EVAL: Repository services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddSingleton(new SqlServerExecutor(configuration.GetConnectionString("SqlConnection")));
            services.AddSingleton<TransactionSqlServerExecutor>();
        }
    }
}
