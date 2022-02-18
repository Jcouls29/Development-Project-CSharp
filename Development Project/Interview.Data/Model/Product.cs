using System;
using System.Collections.Generic;
using System.Text;

namespace Interview.Data.Model
{
    public class Product : BaseModel
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<KeyValuePair<int,string>> ProductAttributes { get; set; }
        public List<int> ProductCategories { get; set; }
    }
}
