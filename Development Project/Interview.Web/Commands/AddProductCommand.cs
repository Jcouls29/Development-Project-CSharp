using Interview.Web.Domain.Dto;
using MediatR;

namespace Interview.Web.Commands;

public record AddProductCommand(ProductData productData) : IRequest<ProductData> {}