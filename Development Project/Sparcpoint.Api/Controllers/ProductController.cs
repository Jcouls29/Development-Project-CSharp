using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Api.Requests;
using Sparcpoint.Core.Entities;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Core.QueryObjects;

namespace Sparcpoint.Api.Controllers;

public static class ProductController
{
    public static void MapProductRoutes(this IEndpointRouteBuilder endpoints)
    {
        const string route = "/api/products";
        var group = endpoints.MapGroup(route);

        group.MapPost("/search", (IProductService productService, [FromBody] ProductSearchQuery? searchQuery) =>
        {
            searchQuery ??= new ProductSearchQuery();

            return productService.SearchProductsAsync(searchQuery);
        });

        group.MapGet("/", (IProductService productService) => productService.SearchProductsAsync(new ProductSearchQuery()));

        // attributes might not be getting saved. Will need to fix relationship
        group.MapGet("/{id:int}", async (IProductService productService, int id) => await productService.GetProductByIdAsync(id));

        group.MapPut("/{id:long?}",
            async Task<Results<Ok<Product>, NotFound>> (IProductService productService, long? id, [FromBody] ProductCreateRequest productCreateRequest) =>
            {
                if (id is not null)
                {
                    var currentProduct = await productService.GetProductByIdAsync(id.Value);

                    if (currentProduct is null)
                    {
                        return TypedResults.NotFound();
                    }

                    currentProduct = currentProduct with
                    {
                        Name = productCreateRequest.Name,
                        Description = productCreateRequest.Description,
                        ImageUris = productCreateRequest.ImageUris,
                        ValidSkus = productCreateRequest.ValidSkus,
                    };

                    if (productCreateRequest.Attributes is not null)
                    {
                        currentProduct = currentProduct with { Attributes = productCreateRequest.Attributes };
                    }

                    if (productCreateRequest.Categories is not null)
                    {
                        // TODO: Get categories by ID
                        // Make sure they exist
                        // Add them to product
                    }

                    await productService.UpdateProductAsync(currentProduct);

                    return TypedResults.Ok(currentProduct);
                }

                var product = new Product
                {
                    Name = productCreateRequest.Name,
                    Description = productCreateRequest.Description,
                    ImageUris = productCreateRequest.ImageUris,
                    ValidSkus = productCreateRequest.ValidSkus,
                };

                if (productCreateRequest.Attributes is not null)
                {
                    product = product with { Attributes = productCreateRequest.Attributes };
                }

                if (productCreateRequest.Categories is not null)
                {
                    // refer to above todo
                }


                await productService.AddProductAsync(product);

                return TypedResults.Ok(product);
            });
    }
}
