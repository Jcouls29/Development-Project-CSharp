using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product : MetadataEntity
    {
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
    }
}
