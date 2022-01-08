using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
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
