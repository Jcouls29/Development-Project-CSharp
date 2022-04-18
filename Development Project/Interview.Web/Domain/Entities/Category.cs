using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities;

public class Category
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [StringLength(FieldLength.Name)]
    public string Name { get; set; }
    [StringLength(FieldLength.Description)]

    public string Description { get; set; }
        
    public ICollection<Product> Products { get; set; }
}