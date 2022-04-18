using System;
using Interview.Web.Domain.Dto;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Domain.Dto;

public class InventoryOperationData
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public Guid InventoryId { get; set; }
    public DateTimeOffset Started { get; set; }
    public DateTimeOffset Completed { get; set; }
}