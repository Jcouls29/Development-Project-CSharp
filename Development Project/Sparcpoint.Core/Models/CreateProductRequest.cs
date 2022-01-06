using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; }
        public List<string> ValidSkus { get; set; }
        public List<KeyValuePair<string,string>> ProductAttributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
