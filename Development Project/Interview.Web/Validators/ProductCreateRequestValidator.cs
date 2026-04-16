using FluentValidation;
using Interview.Web.Models;

namespace Interview.Web.Validators
{
    public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator() 
        {
            this.RuleFor(x => x.Name)
                .NotEmpty();
            
            this.RuleFor(x => x.Description)
                .NotEmpty();
            
            this.RuleFor(x => x.ImageUris)
                .NotEmpty();
            
            this.RuleFor(x => x.ValidSkus)
                .NotEmpty();
            
            this.RuleFor(x => x.Attributes)
                .NotEmpty();
        }
    }
}
