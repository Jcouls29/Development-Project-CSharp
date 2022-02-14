using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class ProductAttributes
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 0)]
        public int InstanceId { get; set; }
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
