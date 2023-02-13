using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Instances].[ProductAttributes]")]
    public class ProductAttribute
    {
        [ExplicitKey]
        public int InstanceId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
