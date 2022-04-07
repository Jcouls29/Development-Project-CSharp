using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBCore
{
    public static class DBRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddDbContext<SparcpointDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbConnection")));

            //services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));

            //services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            //services.AddScoped<IAddressRepository, AddressRepository>();
            //services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            //services.AddScoped<IClientRepository, ClientRepository>();
            //services.AddScoped<IEventRepository, EventRepository>();
            //services.AddScoped<IPaymentRepository, PaymentRepository>();
            return services;
        }
    }
}
