using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DTOs
{
    public class SearchProductRequestDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ValidSku { get; set; }
        public string ProductImageUri { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    }
}
