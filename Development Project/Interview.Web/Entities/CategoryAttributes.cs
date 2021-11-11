using System;

namespace Interview.Web.Entities
{
    public class CategoryAttributes
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Categories Categories { get; set; }
    }
}
