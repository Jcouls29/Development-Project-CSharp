using System;
using System.Collections.Generic;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Domain.Dto;

public class InventoryData
{
    public Guid Id { get; set; }
    public Product Product { get; set; }
    public string ProductInventoryLocation { get; set; }
    public string ProductInventoryDescription { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset Updated { get; set; }
    public DateTimeOffset Created { get; set; }
    public ICollection<InventoryOperationData> InventoryOperations { get; set; }
}