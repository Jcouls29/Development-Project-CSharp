namespace Interview.Web.Models
{
    public class CategoryAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Category Instance { get; set; }
    }
}
