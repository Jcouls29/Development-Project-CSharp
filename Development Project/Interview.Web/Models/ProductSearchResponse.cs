using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class ProductSearchResponse
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> ImageUris { get; set; }

        public IEnumerable<string> Skus { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
