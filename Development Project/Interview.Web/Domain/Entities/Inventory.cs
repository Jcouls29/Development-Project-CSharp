using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities;

public class Inventory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    [StringLength(FieldLength.Location)]
    public string ProductInventoryLocation { get; set; }
    
    [StringLength(FieldLength.Description)]
    public string ProductInventoryDescription { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset Updated { get; set; }
    public DateTimeOffset Created { get; set; }
    public ICollection<InventoryOperation> InventoryOperations { get; set; }
}