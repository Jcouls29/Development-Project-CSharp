using FluentValidation;
using Sparcpoint.Infrastructure.RequestModels;

namespace Sparcpoint.Infrastructure.Fluent
{
    public class ProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.Description).NotEmpty()
                                       .MaximumLength(maximumLength: 120);

            RuleFor(x => x.ProductImageUris).NotEmpty();

            RuleFor(x => x.ValidSkus).NotEmpty()
                                     .Length(min: 3, max: 12);
        }   
    }
}
