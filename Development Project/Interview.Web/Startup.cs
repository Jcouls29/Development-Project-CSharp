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
        services.AddSwaggerGen(c =>
        {
            // EVAL: Wires /// XML comments to Swagger UI so endpoint docs appear at runtime.
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // EVAL: Connection string is config-driven — no code changes needed per environment.
        var sqlOptions = Configuration.GetSection("SqlServer").Get<SqlServerOptions>();
        services.AddSingleton<ISqlExecutor>(new SqlServerExecutor(sqlOptions.ConnectionString));

        // EVAL: Scoped gives each request its own instance — right for DB-backed services.
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
