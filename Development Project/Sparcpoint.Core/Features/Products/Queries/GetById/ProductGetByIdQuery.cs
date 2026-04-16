using MediatR;

namespace Sparcpoint.Features.Products.Queries.GetById
{
    public class ProductGetByIdQuery : IRequest<ProductGetByIdResponse>
    {
        public int Id { get; set; }
    }
}