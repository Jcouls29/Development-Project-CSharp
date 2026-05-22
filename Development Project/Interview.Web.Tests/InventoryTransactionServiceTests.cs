using Interview.Web.Data;
using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Tests;

public class InventoryTransactionServiceTests
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
    public async Task CreateInventoryTransaction_ProductNotFound_ReturnsNull()
    {
        await using var db = CreateContext(nameof(CreateInventoryTransaction_ProductNotFound_ReturnsNull));
        var service = new InventoryTransactionService(db);

        var result = await service.CreateInventoryTransaction(new InventoryTransactionCreateDto
        {
            ProductId = Guid.NewGuid(),
            Quantity = 5,
            Type = InventoryTransactionType.Purchase
        });

        Assert.Null(result);
        Assert.Empty(db.InventoryTransactions);
    }

    [Fact]
    public async Task CreateInventoryTransaction_ValidProduct_PersistsTransaction()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(CreateInventoryTransaction_ValidProduct_PersistsTransaction));
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Widget",
            Description = "Test product",
            Price = 10m
        });
        await db.SaveChangesAsync();

        var service = new InventoryTransactionService(db);

        var result = await service.CreateInventoryTransaction(new InventoryTransactionCreateDto
        {
            ProductId = productId,
            Quantity = 15.5m,
            Type = InventoryTransactionType.Purchase
        });

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.TransactionId);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(15.5m, result.Quantity);
        Assert.Equal(InventoryTransactionType.Purchase, result.Type);
        Assert.NotEqual(default, result.CreatedAt);

        var persisted = await db.InventoryTransactions.SingleAsync();
        Assert.Equal(result.TransactionId, persisted.TransactionId);
        Assert.Equal(productId, persisted.ProductId);
        Assert.Equal(15.5m, persisted.Quantity);
        Assert.Equal(InventoryTransactionType.Purchase, persisted.Type);
    }

    [Fact]
    public async Task GetInventoryTransactionsByProductId_NoTransactions_ReturnsEmpty()
    {
        await using var db = CreateContext(nameof(GetInventoryTransactionsByProductId_NoTransactions_ReturnsEmpty));
        var service = new InventoryTransactionService(db);

        var result = await service.GetInventoryTransactionsByProductId(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInventoryTransactionsByProductId_ReturnsOnlyMatchingProductTransactions()
    {
        var productId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryTransactionsByProductId_ReturnsOnlyMatchingProductTransactions));
        db.InventoryTransactions.AddRange(
            new InventoryTransaction
            {
                ProductId = productId,
                Quantity = 5,
                Type = InventoryTransactionType.Purchase
            },
            new InventoryTransaction
            {
                ProductId = otherProductId,
                Quantity = 99,
                Type = InventoryTransactionType.Purchase
            });
        await db.SaveChangesAsync();

        var service = new InventoryTransactionService(db);

        var result = (await service.GetInventoryTransactionsByProductId(productId)).ToList();

        Assert.Single(result);
        Assert.Equal(productId, result[0].ProductId);
        Assert.Equal(5m, result[0].Quantity);
    }

    [Fact]
    public async Task GetInventoryTransactionsByProductId_ReturnsNewestTransactionsFirst()
    {
        var productId = Guid.NewGuid();
        await using var db = CreateContext(nameof(GetInventoryTransactionsByProductId_ReturnsNewestTransactionsFirst));
        db.InventoryTransactions.AddRange(
            new InventoryTransaction
            {
                ProductId = productId,
                Quantity = 1,
                Type = InventoryTransactionType.Adjustment,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new InventoryTransaction
            {
                ProductId = productId,
                Quantity = 2,
                Type = InventoryTransactionType.Purchase,
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new InventoryTransaction
            {
                ProductId = productId,
                Quantity = 3,
                Type = InventoryTransactionType.Sale,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            });
        await db.SaveChangesAsync();

        var service = new InventoryTransactionService(db);

        var result = (await service.GetInventoryTransactionsByProductId(productId)).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(2m, result[0].Quantity);
        Assert.Equal(3m, result[1].Quantity);
        Assert.Equal(1m, result[2].Quantity);
    }
}
