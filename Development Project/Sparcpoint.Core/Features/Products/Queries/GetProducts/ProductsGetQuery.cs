using MediatR;
using System.Collections.Generic;

namespace Sparcpoint.Features.Products.Queries.GetProducts
{
    public class ProductsGetQuery : IRequest<List<ProductsGetResponseItem>>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public List<Models.AttributeItem> Attributes { get; set; }
    }
}
