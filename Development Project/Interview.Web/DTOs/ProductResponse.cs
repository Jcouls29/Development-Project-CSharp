using System;
using System.Collections.Generic;
using System.Linq;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Response DTO for product data.
    /// Separates the API contract from the internal domain model,
    /// allowing the API shape to evolve independently.
    /// </summary>
    public class ProductResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ProductImageUris { get; set; } = new();
        public List<string> ValidSkus { get; set; } = new();
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();

        /// <summary>
        /// EVAL: Maps domain model to API response DTO.
        /// ProductImageUris and ValidSkus are stored as comma-separated strings in the DB
        /// and deserialized into lists for the API consumer.
        /// </summary>
        public static ProductResponse FromProduct(Product product)
        {
            return new ProductResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = ParseCommaSeparated(product.ProductImageUris),
                ValidSkus = ParseCommaSeparated(product.ValidSkus),
                CreatedTimestamp = product.CreatedTimestamp,
                Attributes = product.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = product.CategoryIds ?? new List<int>()
            };
        }

        private static List<string> ParseCommaSeparated(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<string>();

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
    }
}
