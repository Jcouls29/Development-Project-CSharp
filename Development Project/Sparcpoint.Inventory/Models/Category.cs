using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Instances].[Categories]")]
    public class Category
    {
        [Key]
        public int InstanceId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        [Computed]
        public List<CategoryAttribute> CategoryAttributes { get; set; }

        [Computed]
        public List<CategoryOfCategory> Categories { get; set; }
    }
}