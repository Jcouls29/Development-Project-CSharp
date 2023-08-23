using System.Collections.Generic;

namespace Sparcpoint.Inventory.Domain.Models.Instances
{
    public class ProductSearchModel
    {
        public int InstanceId { get; set; } = int.MinValue;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUris { get; set; } = string.Empty;
        public string ValidSkus { get; set; } = string.Empty;
        public IList<string> Attributes { get; set; } = new List<string>();
        public IList<int> Categories { get; set; } = new List<int>();
    }
}
