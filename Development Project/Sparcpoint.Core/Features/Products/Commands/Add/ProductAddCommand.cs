using MediatR;
using Sparcpoint.Models;
using System.Collections.Generic;

namespace Sparcpoint.Features.Products.Commands.Add
{
    public class ProductAddCommand : IRequest<ProductAddResponse>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }

        public int CategoryId { get; set; }

        public List<AttributeItem> Attributes { get; set; }
    }
}
