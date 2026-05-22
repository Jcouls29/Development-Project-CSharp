using Sparcpoint.Inventory.Repositories.Implementations;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.Inventory.Services.Implementations;
using Sparcpoint.Inventory.Services.Interfaces;
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

        // EVAL: Configure dependency injection for all layers
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // EVAL: Register ISqlExecutor with connection string from configuration
            var connectionString = Configuration.GetConnectionString("InventoryDatabase");
            services.AddSingleton<ISqlExecutor>(sp => new SqlServerExecutor(connectionString));

            // EVAL: Register repositories (scoped for per-request lifecycle)
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            // EVAL: Register services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryService, InventoryService>();

            // EVAL: Add Swagger for API documentation
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Inventory Management API",
                    Version = "v1",
                    Description = "RESTful API for managing products, categories, and inventory transactions"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // EVAL: Enable Swagger UI in development
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API V1");
                    c.RoutePrefix = string.Empty; // Swagger at root
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
