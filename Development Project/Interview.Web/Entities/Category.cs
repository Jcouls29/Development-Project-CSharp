using System;
using System.Collections.Generic;

namespace Interview.Web.Entities
{
    public class Categories
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
