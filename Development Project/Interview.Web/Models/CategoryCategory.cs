using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Models
{
    public partial class CategoryCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Category CategoryInstance { get; set; }
        public virtual Category Instance { get; set; }
    }
}
