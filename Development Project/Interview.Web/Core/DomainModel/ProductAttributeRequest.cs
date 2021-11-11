using System.Runtime.Serialization;

namespace Interview.Web.Core.DomainModel
{
    public class ProductAttributeRequest
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
