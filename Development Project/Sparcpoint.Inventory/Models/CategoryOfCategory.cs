using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Instances].[CategoryCategories]")]
    public class CategoryOfCategory
    {
        [ExplicitKey]
        public int InstanceId { get; set; }

        public int CategoryInstanceId { get; set; }
    }
}