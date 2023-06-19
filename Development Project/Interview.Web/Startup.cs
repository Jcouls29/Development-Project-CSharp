using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Context;
using Sparcpoint.Infrastructure.Services;
using Sparcpoint.Infrastructure.Services.Interfaces;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Interview.Web
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddMvc()
               .AddJsonOptions(
                   options =>
                   {
                       options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                       options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                   }
               );

            // EVAL: Use this to add the JWT authentication for the API endpoints.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            var issuers = Configuration["Authentication:JWTIssuer"].Split(',');
                            var validAudiences = Configuration["Authentication:JWTAudience"].Split(',');

                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidAudiences = validAudiences,
                                ValidIssuers = issuers,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:IssuerSigningKey"])),
                                ClockSkew = TimeSpan.Zero
                            };

                            options.Events = new JwtBearerEvents()
                            {
                                OnAuthenticationFailed = context =>
                            {
                                return context.Response.WriteAsync("Authentication Failed");
                            },
                                OnForbidden = context =>
                            {
                                return context.Response.WriteAsync("Unauthorized");
                            }
                            };
                        });

            // EVAL: Use this to register the connection to database.
            services.AddDbContext<SparcpointBaseContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // EVAL: Use this to register the services.
            services.AddScoped<IProductService, ProductService>();
            // services.AddScoped<ICategoryService, CategoryService>();
            // services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
            // services.AddScoped<IInventoryService, InventoryService>();
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
                    c.SwaggerEndpoint("./v1/swagger.json", "My API V1");
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
            app.UseCors();

            // EVAL: Enable the authentication for the API endpoints.
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}