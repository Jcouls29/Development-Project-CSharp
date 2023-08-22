using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Models{
    public class Category
    {
        [Column("InstanceId")]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}