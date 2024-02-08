using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.SqlServer.Abstractions.Data
{
        public class Product
        {
            public int ProductId { get; set; } // Primary key
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }
}
