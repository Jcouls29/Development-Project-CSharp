using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sparcpoint.Models
{
    public class CategoryOfCategory
    {
        [Key]
        public Guid InstanceId { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryInstanceId { get; set; }
        public Category Category { get; set; }
    }
}
