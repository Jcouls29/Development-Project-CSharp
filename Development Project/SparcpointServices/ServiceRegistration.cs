using Microsoft.Extensions.DependencyInjection;
using SparcpointServices.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using System.Reflection;
using SparcpointServices.Profiles;

namespace SparcpointServices
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddScoped<IProduct, ProductService>();
            services.AddAutoMapper(typeof(SparcpointProfile));
            return services;
        }
    }
}
