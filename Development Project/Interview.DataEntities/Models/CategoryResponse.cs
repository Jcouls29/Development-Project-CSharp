using System;
using System.Collections.Generic;

namespace Interview.DataEntities.Models
{
    public class CategoryResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

    public class CategoryAttributeResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
