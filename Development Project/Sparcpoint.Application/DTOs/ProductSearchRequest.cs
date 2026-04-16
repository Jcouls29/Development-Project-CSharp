using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.DTOs
{
    public class ProductSearchRequest
    {
        public List<int> CategoryIds { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
