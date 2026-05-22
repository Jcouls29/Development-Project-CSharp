using System;
using System.Collections.Generic;

public class ProductResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public Dictionary<string, string> Metadata { get; set; }

    public List<string> Categories { get; set; }
}