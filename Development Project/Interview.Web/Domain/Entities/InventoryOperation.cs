using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities;

public class InventoryOperation
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [StringLength(FieldLength.Description)]
    public string Description { get; set; }
    
    [Required]
    public int Amount { get; set; }
    
    [Required]
    public Guid InventoryId { get; set; }
    
    public Inventory Inventory { get; set; }
    
    [Required]
    public DateTimeOffset Started { get; set; }
    public DateTimeOffset Completed { get; set; }
}