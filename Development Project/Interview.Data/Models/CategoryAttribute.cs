    
namespace Interview.Data.Models
{
    public partial class CategoryAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Category Category { get; set; }
    }
}
