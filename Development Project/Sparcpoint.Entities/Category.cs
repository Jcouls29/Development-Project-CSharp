using System;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Entities
{
    public class Category
    {
        [Key]
        public int InstanceId { get; set; } // In my experience, it is better to use Identity key as long instead of int. We ran into issues with huge tables.

        public DateTime CreatedTimestamp { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }
}