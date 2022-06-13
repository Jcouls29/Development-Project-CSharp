using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Entities
{
    public class Product
    {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
    }
}
