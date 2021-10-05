using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Entities
{
    // EVAL: Entity that represents the CategoryCategory table.
    public partial class CategoryCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Category CategoryInstance { get; set; }
        public virtual Category Instance { get; set; }
    }
}
