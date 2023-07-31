using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Model
{
    public class ProductModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUris { get; set; }
        public string Skus { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Dictionary<string, int> Categories { get; set; }
        public Dictionary<string, object> MetaData { get; set; }
    }
}
