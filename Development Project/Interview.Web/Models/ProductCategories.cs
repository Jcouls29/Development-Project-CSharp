using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class ProductCategories
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 0)]
        public int InstanceId { get; set; }
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int CategoryInstanceId { get; set; }
    }
}
