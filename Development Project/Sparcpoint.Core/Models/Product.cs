using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
    }
}
