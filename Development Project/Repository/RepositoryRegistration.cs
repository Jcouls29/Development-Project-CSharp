using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddPersistenceRepo(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddScoped<IProductRepo, ProductRepo>();
            return services;
        }
    }
}
