using System;
using System.Collections.Generic;

namespace Interview.Service.Models
{
    public class ProductFilterParams
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public Category ProductCategory { get; set; }
        public List<CustAttribute> ProductAttributes { get; set; }
    }
}
