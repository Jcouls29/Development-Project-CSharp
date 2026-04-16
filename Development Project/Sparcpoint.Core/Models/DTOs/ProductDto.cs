using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DTOs
{
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
}
