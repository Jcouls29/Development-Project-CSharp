using System.Collections.Generic;

namespace Interview.Web.Core.DomainModel
{
    public class ProductResponse
    {
       public Entities.Products Products { get; set; }
        public ICollection<ResponseMessage> ResponseMessage { get; set; }
    }
}
