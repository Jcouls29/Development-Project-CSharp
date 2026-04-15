using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Core.DTOs
{
    // EVAL: Data Transfer Object - Used for API communication to avoid exposing domain entities
    // EVAL: Input validation using DataAnnotations for automatic model validation
    public class CreateProductDto
    {
        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Description { get; set; }

        public List<string> ProductImageUris { get; set; } = new List<string>();

        public List<string> ValidSkus { get; set; } = new List<string>();

        public List<int> CategoryInstanceIds { get; set; } = new List<int>();

        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    // EVAL: DTO for product responses - includes computed inventory count
    public class ProductDto
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public DateTime CreatedTimestamp { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public decimal CurrentInventoryCount { get; set; }
    }

    // EVAL: DTO for category operations
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Description { get; set; }

        public List<int> ParentCategoryIds { get; set; } = new List<int>();
    }

    public class CategoryDto
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
        public List<int> ChildCategoryIds { get; set; } = new List<int>();
    }

    // EVAL: DTOs for inventory operations
    public class AddInventoryDto
    {
        [Required]
        [Range(0.000001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [StringLength(32)]
        public string TypeCategory { get; set; }
    }

    public class RemoveInventoryDto
    {
        [Required]
        [Range(0.000001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [StringLength(32)]
        public string TypeCategory { get; set; }
    }

    public class InventoryTransactionDto
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }

    public class InventoryAdjustmentItemDto
    {
        [Required]
        public int ProductInstanceId { get; set; }

        [Required]
        [Range(0.000001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [StringLength(32)]
        public string TypeCategory { get; set; }
    }

    public class InventoryBulkAdjustmentDto
    {
        [Required]
        [MinLength(1)]
        public List<InventoryAdjustmentItemDto> Adjustments { get; set; } = new List<InventoryAdjustmentItemDto>();
    }

    public class InventoryReportRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
        public Dictionary<string, string> MetadataFilters { get; set; } = new Dictionary<string, string>();
    }

    public class InventoryReportDto
    {
        public decimal TotalInventory { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    // EVAL: DTO for search operations - flexible filtering
    public class ProductSearchDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
        public Dictionary<string, string> MetadataFilters { get; set; } = new Dictionary<string, string>();
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }

    public class ProductSearchResultDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int TotalCount { get; set; }
    }
}