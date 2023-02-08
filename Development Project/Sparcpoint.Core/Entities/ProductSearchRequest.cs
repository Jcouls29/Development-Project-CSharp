using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    public class ProductSearchRequest
    {
        public string Keyword { get; set; }

        /// <summary>
        /// Possible options Metadata, Categories, Name, Description, All
        /// </summary>
        /// <example></example>
        public List<string> SearchBy { get; set; }

        //EVAL: In an optimal solution I would want options for order by and order direction as well
        //public string OrderBy { get; set; }
        //public string OrderDirection { get; set; }

        public int Page { get; set; }
        public int PageCount { get; set; }
    }
}