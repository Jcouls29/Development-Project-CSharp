using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; }

        [Required]
        public string Name { get; set; }
        public string description { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Metadata { get; set; } //Json string
        public int OnHand { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CategoryId { get; set; }
    }
}
