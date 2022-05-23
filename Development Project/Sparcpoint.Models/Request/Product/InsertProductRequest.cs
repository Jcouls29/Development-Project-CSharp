using System.Collections.Generic;

namespace Sparcpoint.Models.Request.Product
{
    public class InsertProductRequest
    {
        public List<ProductRequest> Products { get; set; }
    }
}