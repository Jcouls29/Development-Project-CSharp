using System;
using System.Collections.Generic;

namespace Interview.Data.Models
{
    public class Product : ModelBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Sku { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Categories { get; set; }
    }
}