using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Abstract;
using Sparcpoint.Repositories;
using Sparcpoint.SqlServer.Abstractions;

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

            //EVAL: Swagger is a tool for documenting and testing APIs.
            //It provides a user interface to interact with the API endpoints.            
            //The services.AddEndpointsApiExplorer() method is
            //used to enable the API explorer, which allows Swagger to discover and document the API endpoints.
            //The services.AddSwaggerGen() method is used to add the Swagger generator,
            //which generates the Swagger documentation based on the API endpoints
            //defined in the application. This setup allows developers to easily access
            //and test the API endpoints through the Swagger UI.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //EVAL: Configure the dependency injection container to use a SQL Server executor for executing SQL commands.
            //The services.Configure<SqlServerOptions>(Configuration.GetSection("SqlServer")) line is used to
            //bind the SqlServerOptions class to the "SqlServer" section of the application's configuration.
            //This allows the application to read SQL Server-related settings, such as the connection string,
            //from the configuration file (e.g., appsettings.json).
            services.Configure<SqlServerOptions>(Configuration.GetSection("SqlServer"));

            //EVAL: The services.AddScoped<ISqlExecutor>(provider => { ... }) line is used to register a scoped service
            //for the ISqlExecutor interface. This means that a new instance of the SQL Server executor will
            //be created for each HTTP request. The lambda function provided as the second argument is
            //responsible for creating the instance of the SQL Server executor,
            //using the connection string retrieved from the configuration.
            services.AddScoped<ISqlExecutor>(provider =>
            {
                var connectionString = Configuration["SqlServer:ConnectionString"]
                    ?? throw new System.InvalidOperationException("Sql Server ConnectionString configuration is invalid.");
                return new SqlServerExecutor(connectionString);
            });

            //EVAL: The services.AddScoped<IProductRepository, ProductRepository>() line is used to register a scoped service for the IProductRepository interface,
            //which is implemented by the ProductRepository class. This means that a new instance of the ProductRepository will be created for each HTTP request when the IProductRepository interface is requested.
            services.AddScoped<IProductRepository, ProductRepository>();
            //services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
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
