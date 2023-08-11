namespace Interview.Web.Models
{
    public class CategoryCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Category Instance { get; set; }
        public virtual Category CategoryInstance { get; set; }
    }
}
