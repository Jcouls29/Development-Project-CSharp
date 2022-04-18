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

public class AddInventoryHandler : IRequestHandler<AddInventoryCommand, ActionResult<InventoryData>>
{
    private readonly Db _Db;
    private readonly IMapper _Mapper;
    private readonly AddInventoryValidator _Validator;

    public AddInventoryHandler(Db db, IMapper mapper, AddInventoryValidator validator)
    {
        _Db = db;
        _Mapper = mapper;
        _Validator = validator;
    }

    public async Task<ActionResult<InventoryData>> Handle(AddInventoryCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _Validator.ValidateAsync(command);
        if (validationResult.IsValid)
        {
            var inventory = _Mapper.Map<Domain.Entities.Inventory>(command.inventoryData);
            inventory.Created = DateTimeOffset.Now;
            
            inventory.Product = await
                _Db.Products.FindAsync(new object[] { command.inventoryData.Product.Id }, cancellationToken);
            await _Db.Inventories.AddAsync(inventory);
            await _Db.SaveChangesAsync();
            return new OkObjectResult(_Mapper.Map<InventoryData>(inventory));
        }
        else
        {
            return new BadRequestObjectResult(validationResult.Errors);
        }
    }
}