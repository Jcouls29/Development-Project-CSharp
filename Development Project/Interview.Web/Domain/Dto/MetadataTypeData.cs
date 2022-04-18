using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Dto;

public class MetadataTypeData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}