using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> CategoryAttributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
