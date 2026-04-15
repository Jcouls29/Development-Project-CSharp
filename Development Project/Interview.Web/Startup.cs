using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Repositories.Implementations;
using Sparcpoint.Core.Services;
using Sparcpoint.Core.Services.Implementations;
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

        // EVAL: Dependency Injection Configuration - Register all services and repositories
        // EVAL: Clean Architecture - Dependencies point inward toward domain
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Interview.Web API",
                    Version = "v1"
                });
            });

            // EVAL: Infrastructure - SQL Server executor registration
            services.AddSingleton<ISqlExecutor>(provider =>
            {
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                return new SqlServerExecutor(connectionString);
            });

            // EVAL: Repository Layer - Register all repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();

            // EVAL: Service Layer - Register business services
            services.AddScoped<IInventoryService, InventoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview.Web API V1");
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
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger/index.html");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
