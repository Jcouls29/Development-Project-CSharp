using System;

namespace Sparcpoint.API.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ProductImageUris { get; set; }
        public required string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

    public class ProductAttributesModel
    {
        public int InstanceId { get; set; }
        public int Key { get; set; }
        public string? Value { get; set; }
    }

}
