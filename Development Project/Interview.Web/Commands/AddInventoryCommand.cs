using Interview.Web.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Commands;

public record AddInventoryCommand(InventoryData inventoryData) : IRequest<ActionResult<InventoryData>> { }