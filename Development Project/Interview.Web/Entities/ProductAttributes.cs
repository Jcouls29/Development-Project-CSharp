using System;
using System.Runtime.Serialization;

namespace Interview.Web.Entities
{
    public class ProductAttributes
    {
        [IgnoreDataMember]
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        [IgnoreDataMember]
        internal virtual Products Product { get; set; }
    }
}
