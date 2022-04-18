using FluentValidation;
using Interview.Web.Commands;
using Interview.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Validators;

public class AddInventoryOperationValidator : AbstractValidator<AddInventoryOperationCommand>
{
    public AddInventoryOperationValidator(Db db)
    {
        RuleFor(cmd => cmd.inventoryOperationData.InventoryId)
            .NotEmpty()
            .WithMessage("InventoryId required for creating new Inventory Operation.")
            .MustAsync(async (inventoryId, cancellationToken) =>
            {
                var existing = await db.Inventories.FindAsync(new object[] { inventoryId }, cancellationToken);
                return existing != null;
            }).WithMessage("Inventory must exist in the system to add inventory.");
    }
}