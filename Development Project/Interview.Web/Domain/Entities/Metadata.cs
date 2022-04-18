using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities;

public class Metadata
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid MetadataTypeId { get; set; }
    public MetadataType MetadataType { get; set; }
    [StringLength(FieldLength.Name)]

    public string Name { get; set; }
    [StringLength(FieldLength.Description)]

    public string Description { get; set; }
        
    [StringLength(FieldLength.StringValue)]
    public string Value { get; set; }
        
    public List<Product> Products { get; set; }
}