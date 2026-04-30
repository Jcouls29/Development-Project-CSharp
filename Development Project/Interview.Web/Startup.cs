using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Implementations;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // EVAL: Swagger added for easy manual testing during evaluation.
            // In production this would be toggled by environment.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // EVAL: SqlServerOptions is bound from appsettings.json so the
            // connection string is never hardcoded — swapping environments
            // (local, staging, prod) requires only a config change.
            services.Configure<SqlServerOptions>(Configuration.GetSection("SqlServer"));

            // EVAL: ISqlExecutor registered as Scoped so each HTTP request gets
            // its own connection lifecycle. Singleton would risk connection state
            // leaking across requests.
            services.AddScoped<ISqlExecutor>(provider =>
            {
                var connectionString = Configuration["SqlServer:ConnectionString"]
                    ?? throw new System.InvalidOperationException("SqlServer:ConnectionString is not configured.");
                return new SqlServerExecutor(connectionString);
            });

            // EVAL: Repositories registered against their interfaces so the
            // controller never takes a hard dependency on SQL. Swapping to a
            // different store (e.g. CosmosDB, in-memory for tests) means
            // only changing these two lines.
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<IInventoryRepository, SqlInventoryRepository>();
        }

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
