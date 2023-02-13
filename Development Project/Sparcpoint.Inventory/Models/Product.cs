using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Instances].[Products]")]
    public class Product
    {
        [Key]
        public int InstanceId { get; set; }

        // EVAL: these are used as DTOs so we usually don't care since there's no logic involved,
        // but CS8618 warning is disabled at the project level to supress
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        [Computed]
        public List<ProductAttribute> ProductAttributes { get; set; }

        [Computed]
        public List<ProductCategory> Categories { get; set; }

        [Computed]
        public int Inventory { get; set; }
    }
}