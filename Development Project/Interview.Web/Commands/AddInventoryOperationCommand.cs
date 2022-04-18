using Interview.Web.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Commands;

public record AddInventoryOperationCommand(InventoryOperationData inventoryOperationData) : IRequest<ActionResult<InventoryOperationData>> { }