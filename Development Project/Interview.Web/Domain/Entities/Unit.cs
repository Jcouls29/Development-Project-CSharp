using System;

namespace Interview.Web.Domain.Entities;

public class Unit
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
}