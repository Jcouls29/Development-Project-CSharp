using System.ComponentModel.DataAnnotations.Schema;

namespace Inverview.Web.Models
{
    public class ProductCategories
    {
        [Column("InstanceId")]
        public long ProductId { get; set; }
        [Column("CategoryInstanceId")]
        public long CategoryId { get; set; }
    }
}

