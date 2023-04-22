using System.ComponentModel.DataAnnotations.Schema;

namespace Sparcpoint.Core.Entities
{
    [Table("[Instances].[ProductAttributes]")]
    public class ProductAttributes
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}