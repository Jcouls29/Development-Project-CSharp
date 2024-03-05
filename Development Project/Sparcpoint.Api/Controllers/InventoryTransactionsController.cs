using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Api.Requests;
using Sparcpoint.Api.Responses;
using Sparcpoint.Core.Entities;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Core.QueryObjects;

namespace Sparcpoint.Api.Controllers;

public static class InventoryController
{
    public static void MapInventoryTransactionsRoutes(this IEndpointRouteBuilder endpoints)
    {
        const string route = "/api/transactions";
        var group = endpoints.MapGroup(route);

        group.MapGet("/", async (IInventoryTransactionService inventoryTransactionService, [FromQuery] long? productId) =>
        {
            var response = new ProductTransactionResponse();
            if (!productId.HasValue)
            {
                return TypedResults.Ok(response);
            }

            var transactions = new ConcurrentStack<InventoryTransaction>();

            await foreach (var transaction in inventoryTransactionService.GetTransactionsAsync(productId.Value))
            {
                transactions.Push(transaction);
            }

            response.Transactions = transactions.ToList();
            response.TotalQuantity = transactions.Sum(t => t.Quantity);

            return TypedResults.Ok(response);
        });

        group.MapGet("/{id:int}", async (IInventoryTransactionService inventoryTransactionService, int id) => await inventoryTransactionService.GetTransactionByIdAsync(id));

        group.MapPost("/search", async (IInventoryTransactionService inventoryTransactionService, IProductService productService, [FromBody] ProductSearchQuery? searchQuery) =>
        {
            var response = new ProductTransactionResponse();

            searchQuery ??= new ProductSearchQuery();

            var products = productService.SearchProductsAsync(searchQuery);
            var productIds = new ConcurrentStack<long>();
            var transactions = new ConcurrentStack<InventoryTransaction>();

            await foreach (var product in products)
            {
                if (productIds.Contains(product.Id))
                {
                    continue;
                }

                productIds.Push(product.Id);
            }

            await foreach (var transaction in inventoryTransactionService.GetTransactionsAsync(productIds.ToList()))
            {
                transactions.Push(transaction);
            }

            response.Transactions = transactions.ToList();
            // right now this is for all products. will need to separate this out
            response.TotalQuantity = transactions.Sum(t => t.Quantity);

            return TypedResults.Ok(response);
        });

        group.MapPost("/product",
            async Task<Results<Ok<InventoryTransaction>, NotFound, BadRequest<string>>> (IInventoryTransactionService inventoryTransactionService, IProductService productService, [FromBody] ProductTransactionRequest createRequest) =>
        {
            var product = await productService.GetProductByIdAsync(createRequest.ProductId);

            if (product is null)
            {
                return TypedResults.NotFound();
            }

            if (createRequest.Quantity <= 0)
            {
                return TypedResults.BadRequest("Quantity must be greater than 0");
            }

            var transactions = await inventoryTransactionService.AddProductTransactionAsync([
                new ValueTuple<long, decimal>(createRequest.ProductId, createRequest.Quantity)
            ]);

            return TypedResults.Ok(transactions.FirstOrDefault());
        });

        group.MapPost("/products",
            async Task<Results<Ok<List<InventoryTransaction>>, NotFound, BadRequest<string>>> (IInventoryTransactionService inventoryTransactionService, IProductService productService, [FromBody] ProductTransactionRequests createRequests) =>
        {
            foreach (var productCreateRequest in createRequests.Products)
            {
                var product = await productService.GetProductByIdAsync(productCreateRequest.ProductId);

                if (product is null)
                {
                    return TypedResults.NotFound();
                }

                if (productCreateRequest.Quantity <= 0)
                {
                    return TypedResults.BadRequest("Quantity must be greater than 0");
                }
            }

            var productTuples = createRequests.Products.Select(p => (p.ProductId, p.Quantity)).ToList();

            var transactions = await inventoryTransactionService.AddProductTransactionAsync(productTuples);

            return TypedResults.Ok(transactions);
        });
    }
}
