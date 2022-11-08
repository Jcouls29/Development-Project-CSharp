using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.Products
{
    internal class ValidationRules
    {

        internal class DefaultProductValidator : AbstractValidator<BaseProduct>
        {
            internal DefaultProductValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(255).WithMessage("Name cannot be empty");
                RuleFor(x => x.Description).NotEmpty().MaximumLength(255);
            }
        }

        //Example for specific client

        internal class Consumer1ProductValidator : AbstractValidator<BaseProduct>
        {
            internal Consumer1ProductValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(255).WithMessage("Name cannot be empty");
                RuleFor(x => x.Description).NotEmpty().MaximumLength(255);
                RuleFor(x => x.ValidSkus).NotEmpty();
            }
        }
    }
}
