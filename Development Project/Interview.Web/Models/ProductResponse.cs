using System;
using System.Collections.Generic;

namespace Interview.Web.Models;

public class ProductResponse
{
    public int InstanceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] ValidSkus { get; set; }
    public string[] ProductImageUris { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
}