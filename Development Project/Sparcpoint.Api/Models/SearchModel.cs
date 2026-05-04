using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.API.Models
{
    public class SearchModel
    {
        public int Id { get; set; }
        public string? Query { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
    }

    public class ReturnModel
    {
        public List<int>? ProductIds { get; set; }
    }
}
