using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Models
{
    public class ProductAttributes 
    {
        [Column("InstanceId")]
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}