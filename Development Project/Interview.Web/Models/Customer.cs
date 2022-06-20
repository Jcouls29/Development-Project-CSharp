using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    }
}
