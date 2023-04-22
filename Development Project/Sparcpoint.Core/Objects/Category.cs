using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sparcpoint.Core.Entities
{
    [Table("[Instances].[Categories]")]
    public class Category
    {
        [Key]
        public int InstanceId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}