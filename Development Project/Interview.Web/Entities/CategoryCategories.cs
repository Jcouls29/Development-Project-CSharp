using System;

namespace Interview.Web.Entities
{
    public class CategoryCategories
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }

        public virtual Categories CategoryInstance { get; set; }
        public virtual Categories Instance { get; set; }
    }
}
