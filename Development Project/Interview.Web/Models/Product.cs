using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<string> Attributes { get; set; } = new List<string>();
    }
}
