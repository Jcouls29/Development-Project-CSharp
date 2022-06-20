using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
