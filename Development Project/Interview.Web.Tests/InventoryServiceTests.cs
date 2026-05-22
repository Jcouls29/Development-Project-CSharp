using Interview.Web.Data;
using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Tests;

public class InventoryServiceTests
{
    private static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetInventoryByProductId_ProductNotFound_ReturnsNull()
    {
        await using var db = CreateContext(nameof(GetInventoryByProductId_ProductNotFound_ReturnsNull));
        var service = new InventoryService(db);

        var result = await service.GetInventoryByProductId(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetInventoryByProductId_NoTransactions_ReturnsZero()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryByProductId_NoTransactions_ReturnsZero));

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Widget",
            Description = "Test",
            Price = 1
        });
        await db.SaveChangesAsync();

        var service = new InventoryService(db);

        var result = await service.GetInventoryByProductId(productId);

        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(0m, result.OnHandQuantity);
    }

    [Fact]
    public async Task GetInventoryByProductId_CalculatesAdjustmentPurchaseMinusSale()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryByProductId_CalculatesAdjustmentPurchaseMinusSale));

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Widget",
            Description = "Test",
            Price = 1
        });
        db.InventoryTransactions.AddRange(
            new InventoryTransaction { ProductId = productId, Quantity = 5, Type = InventoryTransactionType.Adjustment },
            new InventoryTransaction { ProductId = productId, Quantity = 10, Type = InventoryTransactionType.Purchase },
            new InventoryTransaction { ProductId = productId, Quantity = 3, Type = InventoryTransactionType.Sale });
        await db.SaveChangesAsync();

        var service = new InventoryService(db);

        var result = await service.GetInventoryByProductId(productId);

        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(12m, result.OnHandQuantity);
    }

    [Fact]
    public async Task GetInventoryByProductId_SalesOnly_SubtractsFromZero()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryByProductId_SalesOnly_SubtractsFromZero));

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Widget",
            Description = "Test",
            Price = 1
        });
        db.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            Quantity = 7,
            Type = InventoryTransactionType.Sale
        });
        await db.SaveChangesAsync();

        var service = new InventoryService(db);

        var result = await service.GetInventoryByProductId(productId);

        Assert.NotNull(result);
        Assert.Equal(-7m, result.OnHandQuantity);
    }

    [Fact]
    public async Task GetInventoryByProductId_IgnoresOtherProducts()
    {
        var productId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryByProductId_IgnoresOtherProducts));

        db.Products.AddRange(
            new Product { Id = productId, Name = "Widget", Description = "Test", Price = 1 },
            new Product { Id = otherProductId, Name = "Other", Description = "Test", Price = 1 });
        db.InventoryTransactions.AddRange(
            new InventoryTransaction { ProductId = productId, Quantity = 5, Type = InventoryTransactionType.Purchase },
            new InventoryTransaction { ProductId = otherProductId, Quantity = 100, Type = InventoryTransactionType.Purchase });
        await db.SaveChangesAsync();

        var service = new InventoryService(db);

        var result = await service.GetInventoryByProductId(productId);

        Assert.NotNull(result);
        Assert.Equal(5m, result.OnHandQuantity);
    }
}
