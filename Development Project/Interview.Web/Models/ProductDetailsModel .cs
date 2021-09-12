using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.models
{
    public class ProductDetailsModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductProductImageUris { get; set; }
        public string ProductValidSkus { get; set; }
        public Boolean Active { get; set; }
    }
}
