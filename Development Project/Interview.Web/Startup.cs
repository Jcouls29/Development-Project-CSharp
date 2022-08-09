using Interview.Web.Extension;
using Interview.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Sparcpoint;
using Sparcpoint.BusinessLayer.Product;
using Sparcpoint.BusinessLayer.User;
using Sparcpoint.DataLayer.Context;
using Sparcpoint.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SparcPoint Api",
                    Version = "v1"
                });
            });
            services.AddControllers();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductLayer, ProductLayer>();
            services.AddScoped<IDataSerializer,JsonDataSerializer>();
            services.AddScoped<IProductDBContext, ProductDBContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ModelValidationAttribute>();
            //AUthorization token for calling the endpoint
            services.ConfigureAuth(Configuration);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SparcPOint Api");
            });
            app.UseExceptionMiddleware();
            app.UseHsts();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
