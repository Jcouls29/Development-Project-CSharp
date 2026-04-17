using Interview.Application.Abstractions;
using Interview.Application.UseCases.Query;
using Interview.Application.UseCases.Result;
using Interview.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchProductsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchProducts(
        [FromBody] SearchProductsRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var outcome = await _productService
            .SearchProductsAsync(request.ToQuery(), cancellationToken)
            .ConfigureAwait(false);

        return outcome switch
        {
            SearchProductsOutcomeResult.Succeeded success => Ok(ToResponse(success.Result)),
            SearchProductsOutcomeResult.InvalidPagination => BadRequest(new
            {
                error = $"Page must be at least 1 and PageSize must be between 1 and {SearchProductsConstraints.MaxPageSize}.",
            }),
            SearchProductsOutcomeResult.InvalidQuery => BadRequest(new { error = "One or more search filters are invalid." }),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    private static SearchProductsResponse ToResponse(SearchProductsResult result)
    {
        var items = result.Items
            .Select(p => new ProductSummaryResponse
            {
                InstanceId = p.InstanceId,
                Name = p.Name,
                Description = p.Description,
            })
            .ToList();

        return new SearchProductsResponse
        {
            Items = items,
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var outcome = await _productService.CreateAsync(command, cancellationToken).ConfigureAwait(false);

        return outcome switch
        {
            CreateProductResult.Succeeded success => Created(
                $"/api/v1/products/{success.ProductInstanceId}",
                new CreateProductResponse { InstanceId = success.ProductInstanceId }),
            CreateProductResult.InvalidName => BadRequest(new { error = "Name must not be empty or whitespace." }),
            CreateProductResult.DuplicateName => Conflict(new { error = "A product with this name already exists." }),
            CreateProductResult.DuplicateAttributeKeys duplicateKeys => BadRequest(new
            {
                error = "Duplicate attribute keys in the request.",
                duplicateKeys = duplicateKeys.Keys,
            }),
            CreateProductResult.InvalidCategories invalid => BadRequest(new
            {
                error = "One or more category identifiers were not found.",
                missingCategoryIds = invalid.MissingCategoryIds,
            }),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
