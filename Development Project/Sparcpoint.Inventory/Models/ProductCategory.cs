using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Instances].[ProductCategories]")]
    public class ProductCategory
    {
        public int InstanceId { get; set; }

        [ExplicitKey]
        public int CategoryInstanceId { get; set; }
    }
}
