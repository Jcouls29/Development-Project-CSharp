using Dapper.Contrib.Extensions;

namespace Sparcpoint.Models.Tables
{
    [Table("[Instances].[ProductCategories]")]
    public class ProductCategory
    {
        [ExplicitKey]
        public int InstanceId { get; set; }
        [ExplicitKey]
        public int CategoryInstanceId { get; set; }
    }
}