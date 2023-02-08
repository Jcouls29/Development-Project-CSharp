using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Dtos
{
    // EVAL: Dtos help us prevent mass assigment attacks
    public class ProductDto
    {
        public int InstanceId { get; set; }

        [Required(ErrorMessage = "Name is mandatory field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is mandatory field")]
        public string Description { get; set; }
    }
}
