using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.Json.Serialization;
using Interview.Web.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;

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
            services.AddSingleton(Configuration.GetSection("ProductInventoryConfiguration").Get<ProductInventoryConfiguration>());

            // services.Configure<ProductInventoryConfiguration>(Configuration.GetSection("ProductInventoryConfiguration"));

            services.AddControllers().AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
                
                //.AddJsonOptions(x =>
                //x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            
            var connectionString = Configuration.GetConnectionString("ProductInventory");
            services.AddSqlite<Db>(connectionString);
            
            //services.AddDbContext<Db>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IProductInvetoryDb, Db>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Product Inventory Management API",
                    Description = "An ASP.NET Core Web API for managing Product Inventory",
                    Contact = new OpenApiContact
                    {
                        Name = "Pawel Kaniewski",
                        Email = "b1ker@fastmail.com"
                    }
                });
            });
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            using var scope = app.ApplicationServices.CreateScope();
            var productInventoryConfiguration = app.ApplicationServices.GetRequiredService<ProductInventoryConfiguration>();

            var context = scope.ServiceProvider.GetRequiredService<Db>();
            var defaultTimeout = context.Database.GetCommandTimeout();
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

            if (productInventoryConfiguration.RunMigrationsOnStartUp)
            {
                logger.LogInformation("Running migrations on database");
                context.Database.Migrate();
                logger.LogInformation("Completed running migrations on database");
            }
            else
            {
                logger.LogWarning("Skipping migrations on database");
            }

            context.Database.SetCommandTimeout(defaultTimeout);
        }
    }
}
