using Interview.Web.Repositories.Implementations;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Implementations;
using Interview.Web.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web;

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
        services.AddSwaggerGen();

        // EVAL: SqlServerOptions is bound from appsettings.json so the connection string
        // is configurable per environment without code changes (Open/Closed principle).
        var sqlOptions = Configuration.GetSection("SqlServer").Get<SqlServerOptions>();
        services.AddSingleton<ISqlExecutor>(new SqlServerExecutor(sqlOptions.ConnectionString));

        // EVAL: Scoped lifetime for repository and service means one instance per HTTP request,
        // which is appropriate for database-backed services.
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryService, InventoryService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview API v1");
            });
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
