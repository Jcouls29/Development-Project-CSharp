using Microsoft.EntityFrameworkCore;
using Sparcpoint.Api.Controllers;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Infrastructure.Data;
using Sparcpoint.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlite(
        "Data Source=sparcpoint.db",
        x => x.MigrationsAssembly("Sparcpoint.Infrastructure")
        );
});
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapProductRoutes();
app.MapInventoryTransactionsRoutes();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await dbContext.Database.MigrateAsync();

app.Run();

// used only for integration tests
public partial class Program
{

}
