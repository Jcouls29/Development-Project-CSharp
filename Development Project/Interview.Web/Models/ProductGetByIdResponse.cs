using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class ProductGetByIdResponse
    {
        public int InstanceId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public IEnumerable<string> ImageUris { get; set; }

        public IEnumerable<string> ValidSkus { get; set; }

        public IDictionary<string, string> Attributes { get; set; }
    }
}