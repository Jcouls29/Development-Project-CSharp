using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Application.Implementations;
using Sparcpoint.Domain.Profiles;
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
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ProductProfile), typeof(InventoryTransactionProfile));
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddSingleton(Configuration.GetSection("SqlServerOptions").Get<SqlServerOptions>());
            services.AddScoped<ISqlExecutor, SqlServerExecutor>();
            services.AddScoped<IQueryService, QueryService>();
            services.AddScoped<SqlServerQueryProvider, SqlServerQueryProvider>();
            services.AddScoped<ISqlExecutor, SqlServerExecutor>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryValidationService, CategoryValidationService>();
            services.AddScoped<IProductValidationService, ProductValidationService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInventoryValidationService, InventoryValidationService>();

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
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI();
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
