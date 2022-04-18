using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities;

public class MetadataType
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [StringLength(FieldLength.Name)]

    public string Name { get; set; }
}