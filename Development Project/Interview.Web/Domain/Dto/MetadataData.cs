using System;

namespace Interview.Web.Domain.Dto;

public class MetadataData
{
    public Guid Id { get; set; }
    public MetadataTypeData MetadataType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
}