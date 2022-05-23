using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Entities
{
    public class ProductCategory
    {
        //EVAL: I see InstanceId references ProductInstanceId, Instead I would create a new column as Identity Column InstanceId
        [Key]
        public int InstanceId { get; set; }

        public int CategoryInstanceId { get; set; }

        public int ProductInstanceId { get; set; }
    }
}