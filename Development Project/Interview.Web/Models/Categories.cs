using System;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class Categories
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateTimestamp { get; set; }
    }
}
