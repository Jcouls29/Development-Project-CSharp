using Sparcpoint.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Dto
{
    public class InventoryTransactionDto
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

    }

    public class ProductDto
    {
       
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        public string ProductImageUris { get; set; }

        [Required]
        public string ValidSkus { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

    }


    public class CategoryDto
    {
     

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        
    }


    public enum CountType
    {
        PRODUCTID,
        METADATA
    }
}
