using Interview.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddSingleton(Configuration.GetSection("SqlServerOptions").Get<SqlServerOptions>());
            // EVAL: alternatively use built-in ConnectionStrings section if preferred:
            //var connectionString = Configuration.GetConnectionString("SqlConnection");
            //services.AddSingleton(new SqlServerOptions { ConnectionString = connectionString });

            services.AddScoped<ISqlExecutor, SqlServerExecutor>();
            services.AddScoped<IProductService, ProductService>();
            // EVAL: add additional services once completed, any validators, etc.
            //services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IInventoryService, InventoryService>();

            services.AddAutoMapper(
                typeof(CategoryProfile),
                typeof(InventoryTransactionProfile),
                typeof(ProductProfile)
            );

            services.AddControllers();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
