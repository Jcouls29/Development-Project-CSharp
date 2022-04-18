using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Interview.Web.Commands;
using Interview.Web.Domain.Dto;
using Interview.Web.Infrastructure;
using Interview.Web.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Handlers.Inventory;

public class AddInventoryOperationHandler : IRequestHandler<AddInventoryOperationCommand, ActionResult<InventoryOperationData>>
{
    private readonly Db _Db;
    private readonly IMapper _Mapper;
    private readonly AddInventoryOperationValidator _Validator;

    public AddInventoryOperationHandler(Db db, IMapper mapper, AddInventoryOperationValidator validator)
    {
        _Db = db;
        _Mapper = mapper;
        _Validator = validator;
    }

    public async Task<ActionResult<InventoryOperationData>> Handle(AddInventoryOperationCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _Validator.ValidateAsync(command);
        if (validationResult.IsValid)
        {
            var inventoryOperation = _Mapper.Map<Domain.Entities.InventoryOperation>(command.inventoryOperationData);
            inventoryOperation.Started = inventoryOperation.Completed = DateTimeOffset.Now;

            inventoryOperation.Inventory = await
                _Db.Inventories.FindAsync(new object[] { command.inventoryOperationData.InventoryId }, cancellationToken);
            inventoryOperation.Inventory.Quantity += command.inventoryOperationData.Amount;
            inventoryOperation.Inventory.Updated = DateTimeOffset.Now;

            await _Db.InventoryOperations.AddAsync(inventoryOperation);
            await _Db.SaveChangesAsync();
            return new OkObjectResult(_Mapper.Map<InventoryOperationData>(inventoryOperation));
        }
        else
        {
            return new BadRequestObjectResult(validationResult.Errors);
        }
    }
}