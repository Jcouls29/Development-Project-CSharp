using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ProductImageUris { get; private set; } = new List<string>();
        public List<string> ValidSkus { get; private set; } = new List<string>();
        public List<Category> Categories { get; private set; } = new List<Category>();
        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();
    }
}
