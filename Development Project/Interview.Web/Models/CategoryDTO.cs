using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Models
{
    [Table("CategoryAttributes")]
    public class CategoryDTO
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime CreatedDateStamp { get; set; }
    }
}
