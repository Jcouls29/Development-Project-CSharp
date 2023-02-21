using Lamar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sparcpoint.InventoryService.Repository.Extensions;
using Polly;

namespace Interview.Web
{
    public class Startup
    {
        private IWebHostEnvironment _hostingEnvironment;
        private static ILogger _logger;
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory logFactory)
        {
            var _builder = new ConfigurationBuilder();
            _builder.SetBasePath(hostingEnvironment.ContentRootPath)
                   .AddEnvironmentVariables()
                   .AddJsonFile(hostingEnvironment.ContentRootPath + @"/config/appsettings/appsettings.json", optional: false)
                   .AddJsonFile(hostingEnvironment.ContentRootPath + @"/config/appsettings/" + $"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true);

            Configuration = _builder.Build();
            //Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();

            // Custom auth policies

            // Add framework services
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddControllersAsServices()
              //EVAL: TODO: Ricardo Add fluent validation
              /*.AddFluentValidation(fv =>
              {
              })*/;

            // Configuration
            services.AddSingleton(Configuration);

            // Swagger UI
            ConfigureSwaggerUI(services);

            ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //EVAL: Configure Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service API");
                });
                // Set Swagger UI as the service's home page
                var option = new RewriteOptions();
                option.AddRedirect("^$", "swagger");
                app.UseRewriter(option);
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

        private void ConfigureSwaggerUI(ServiceRegistry services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddSwaggerGen();
        }

        public void ConfigureDependencyInjection(ServiceRegistry services)
        {
            var handlerAssembly = Assembly.Load("Sparcpoint.Inventory.Handler");
            services.AddMediatR(handlerAssembly);

            var builder = services.BuildServiceProvider();
            //TODO: Ricardo
            /*var applicationConfigMonitor = builder.GetService<IOptionsMonitor<ApplicationConfig>>();
            
            if (applicationConfigMonitor == null || applicationConfigMonitor.CurrentValue == null)
            {
                Console.WriteLine($"Encountered problem retrieving applicationSettings. Environment: {_hostingEnvironment?.EnvironmentName}");
            }*/

            _logger = builder.GetService<ILogger>();

            //TODO: Ricardo
            //services.AddAuthorization(Configuration);

            //services.AddRepositories(applicationConfigMonitor, _logger);
            services.AddRepositories(Configuration, _logger);

            services.Scan(x =>
            {
                x.AssemblyContainingType(typeof(Startup));
                x.WithDefaultConventions();
            });
        }
    }
}
