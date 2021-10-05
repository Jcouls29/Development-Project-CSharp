using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Entities
{
    // EVAL: Entity that represents the ProductCategory table.
    public partial class ProductCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Category CategoryInstance { get; set; }
        public virtual Product Instance { get; set; }
    }
}
