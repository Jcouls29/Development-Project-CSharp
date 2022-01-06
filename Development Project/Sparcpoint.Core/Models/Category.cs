using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
