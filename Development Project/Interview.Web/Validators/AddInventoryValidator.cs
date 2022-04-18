using FluentValidation;
using Interview.Web.Commands;
using Interview.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Validators;

public class AddInventoryValidator : AbstractValidator<AddInventoryCommand>
{
    public AddInventoryValidator(Db db)
    {
        RuleFor(cmd => cmd.inventoryData.Product.Id)
            .NotEmpty()
            .WithMessage("ProductId required for creating new Inventory.")
            .MustAsync(async (productId, cancellationToken) =>
            {
                var existing = await db.Products.FindAsync(new object[] { productId }, cancellationToken);
                return existing != null;
            }).WithMessage("Product must exist in the system to add inventory.");
    }
}