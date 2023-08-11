namespace Interview.Web.Models
{
    public class ProductCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Product Instance { get; set; }
        public virtual Category CategoryInstance { get; set; }
    }
}
