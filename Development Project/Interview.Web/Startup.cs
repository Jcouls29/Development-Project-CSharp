using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Reflection;
using Domain.Interface;
using Domain.Entity;
using Repository.Interfaces;
using Repository.Repositories;
using Domain.Dtos;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;
using Microsoft.AspNet.OData.Extensions;
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

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IInventoryRepository, InventoryRepository>();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            var assembly = AppDomain.CurrentDomain.Load("Application");
            services.AddMediatR(assembly);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("Product-Management-API", new OpenApiInfo
                {
                    Title = "Proudct Management  ",
                    Version = "v1"
                });

                c.IncludeXmlComments("Swagger.XML.Comments.xml");
                c.CustomSchemaIds(x => x.FullName);
            });
            services.AddControllers();
            services.AddRouting();
            services.AddOData();
            

        }
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<ProductDto>("Products");
            return builder.GetEdmModel();
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


          //  app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            //_ = app.UseEndpoints(endpoints =>
            //{
            //    _ = endpoints.MapODataRoute("odata", "odata", (Microsoft.OData.Edm.IEdmModel)GetEdmModel());
            //});
            
          

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/Product-Management-API/swagger.json", "Product-Management-API");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
