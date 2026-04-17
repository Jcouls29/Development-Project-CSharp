using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Services;
using Interview.Web.Repositories;
using Interview.Web.Middleware;

namespace Interview.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<Sparcpoint.SqlServer.Abstractions.ISqlExecutor>(sp => 
            { 
                var conn = Configuration.GetConnectionString("DefaultConnection"); 
                return new Sparcpoint.SqlServer.Abstractions.SqlServerExecutor(conn); 
            }); 

            services.AddSingleton<Interview.Web.Repositories.IProductRepository, Interview.Web.Repositories.SqlProductRepository>();
            services.AddSingleton<IProductService, ProductService>();

            services.AddSingleton<Interview.Web.Repositories.ICategoryRepository, Interview.Web.Repositories.SqlCategoryRepository>();
            services.AddSingleton<ICategoryService, CategoryService>();

            services.AddSingleton<Interview.Web.Repositories.IInventoryRepository, Interview.Web.Repositories.SqlInventoryRepository>();
            services.AddSingleton<IInventoryService, InventoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Global middlewares
            app.UseErrorHandling();
            app.UseRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
