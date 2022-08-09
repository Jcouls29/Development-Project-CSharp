using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class FilterParam
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUri { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; }
    }
}
