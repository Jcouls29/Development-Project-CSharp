using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Entities
{
    // EVAL: Entity that represents the CategoryAttribute table.
    public partial class CategoryAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Category Instance { get; set; }
    }
}
