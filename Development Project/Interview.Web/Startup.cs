using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.DataServices;
using Sparcpoint.Services;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
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
            var servicesProvider = services.BuildServiceProvider();
            var configuration = servicesProvider.GetService<IConfiguration>();

            services.AddControllers();
            services.AddSwaggerGen(g =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Interview.Web.xml");
                g.IncludeXmlComments(filePath);
            });

            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IProductDataService>(ds => {
                var connString = configuration.GetValue<string>("ProductDBConn");
                return new ProductDataService(connString);
            });
            services.AddSingleton<ICategoryDataService>(ds => {
                var connString = configuration.GetValue<string>("ProductDBConn");
                return new CategoryDataService(connString);
            });
            services.AddSingleton<IInventoryDataService>(ds => {
                var connString = configuration.GetValue<string>("ProductDBConn");
                return new InventoryDataService(connString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
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
