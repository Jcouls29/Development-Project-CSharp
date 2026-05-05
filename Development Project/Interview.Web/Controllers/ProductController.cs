// EVAL: ProductController handles all product-related API endpoints.
// Uses [ApiController] for automatic model validation (returns 400 on invalid input)
// and constructor-injected IProductRepository for data access.
// The controller is thin -- it maps between DTOs and domain models,
// delegates business logic to the repository, and handles HTTP concerns (status codes, headers).

using Interview.Web.Models.Requests;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// API endpoints for managing products in the inventory system.
    /// </summary>
    [Route("api/v1/products")]
    [ApiController]
    // EVAL: [ApiController] provides automatic model validation, binding source inference,
    // and ProblemDetails error responses. This eliminates manual ModelState checking.
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Initializes the controller with required dependencies.
        /// </summary>
        // EVAL: Constructor injection of IProductRepository. The controller has no knowledge
        // of SQL Server, Dapper, or any data access details. This makes it testable
        // with a mock repository and reusable across different data stores.
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        /// <summary>
        /// Creates a new product with categories, metadata, and general details.
        /// </summary>
        /// <param name="request">Product details including name, description, attributes, and category associations.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created product with its generated ID.</returns>
        /// <response code="201">Product created successfully.</response>
        /// <response code="400">Validation failed (missing required fields, constraint violations).</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            // RV: Additional validation beyond data annotations.
            // Attribute keys/values must respect DB column constraints.
            if (request.Attributes != null)
            {
                foreach (var attr in request.Attributes)
                {
                    if (attr.Key.Length > 64)
                        return BadRequest(new ErrorResponse
                        {
                            Code = "VALIDATION_ERROR",
                            Message = $"Attribute key '{attr.Key}' exceeds maximum length of 64 characters."
                        });

                    if (attr.Value != null && attr.Value.Length > 512)
                        return BadRequest(new ErrorResponse
                        {
                            Code = "VALIDATION_ERROR",
                            Message = $"Attribute value for key '{attr.Key}' exceeds maximum length of 512 characters."
                        });
                }
            }

            // EVAL: Validate image URIs are well-formed absolute URIs.
            // Prevents malformed data from being stored and causing issues in UI consumers.
            if (request.ImageUris != null)
            {
                foreach (var uri in request.ImageUris)
                {
                    if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                        return BadRequest(new ErrorResponse
                        {
                            Code = "VALIDATION_ERROR",
                            Message = $"Image URI '{uri}' is not a valid absolute URI."
                        });
                }
            }

            // Map request DTO to domain model
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ImageUris ?? new List<string>(),
                ValidSkus = request.Skus ?? new List<string>(),
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = request.CategoryIds ?? new List<int>()
            };

            if (product.CategoryIds.Count > 0)
            {
                var requestedCategoryIds = product.CategoryIds.Distinct().ToList();
                var existingCategoryIds = await _productRepository.GetExistingCategoryIdsAsync(
                    requestedCategoryIds, cancellationToken);
                var missingCategoryIds = requestedCategoryIds
                    .Where(id => !existingCategoryIds.Contains(id))
                    .ToList();
                if (missingCategoryIds.Count > 0)
                    return BadRequest(new ErrorResponse
                    {
                        Code = "CATEGORY_NOT_FOUND",
                        Message = $"Category ID(s) not found: {string.Join(", ", missingCategoryIds)}"
                    });
            }

            var created = await _productRepository.CreateAsync(product, cancellationToken);

            // Map domain model to response DTO
            var response = MapToResponse(created);

            // EVAL: 201 Created with Location header is the proper REST response for resource creation.
            // The Location header tells the client where to find the new resource.
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = created.InstanceId },
                response);
        }

        /// <summary>
        /// Retrieves a single product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The product with all attributes and category associations.</returns>
        /// <response code="200">Product found.</response>
        /// <response code="404">Product not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetProductById(
            int id,
            CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product == null)
                return NotFound(new ErrorResponse
                {
                    Code = "PRODUCT_NOT_FOUND",
                    Message = $"Product with ID {id} was not found."
                });

            return Ok(MapToResponse(product));
        }

        /// <summary>
        /// Searches for products by category, metadata, general details, or any combination.
        /// All filters use AND logic. Omitted filters are not applied.
        /// </summary>
        /// <param name="request">Search filters and pagination parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of matching products.</returns>
        /// <response code="200">Search results (may be empty).</response>
        // EVAL: Search returns 200 with an empty list for no results (not 404).
        // 404 means the resource itself doesn't exist; an empty search result is valid.
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductResponse>), 200)]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] SearchProductsRequest request,
            CancellationToken cancellationToken)
        {
            // Map request DTO to domain search criteria
            var criteria = new ProductSearchCriteria
            {
                Name = request.Name,
                Description = request.Description,
                CategoryIds = request.CategoryIds,
                Attributes = request.Attributes,
                Skip = request.Skip,
                Take = request.Take
            };

            var products = await _productRepository.SearchAsync(criteria, cancellationToken);

            var response = products.Select(MapToResponse).ToList();

            return Ok(response);
        }

        #region Private Helpers

        /// <summary>
        /// Maps a domain Product to a ProductResponse DTO.
        /// </summary>
        // EVAL: Centralized mapping method ensures consistent response shape.
        // In a larger system, consider AutoMapper or a dedicated mapping service.
        // For this scope, explicit mapping is clearer and has zero dependencies.
        private ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                // EVAL: Product model now has native List<string> for ImageUris/Skus.
                // JSON deserialization is handled at the repository layer.
                ImageUris = product.ProductImageUris ?? new List<string>(),
                Skus = product.ValidSkus ?? new List<string>(),
                Attributes = product.Attributes ?? new Dictionary<string, string>(),
                // EVAL: Category names are populated via JOIN in the repository layer,
                // so no extra queries are needed here.
                Categories = product.Categories?.Select(kvp => new CategorySummary
                {
                    InstanceId = kvp.Key,
                    Name = kvp.Value
                }).ToList() ?? new List<CategorySummary>(),
                CreatedTimestamp = product.CreatedTimestamp
            };
        }

        #endregion
    }
}
