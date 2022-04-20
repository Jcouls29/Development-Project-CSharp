using Dapper.Contrib.Extensions;
using System;
namespace Interview.Web.Models
{
  [Table("Instances.Products")]
  public class Product
  {
    [Key]
    public int InstanceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProductImageUris { get; set; }
    public string ValidSkus { get; set; }
    public DateTime CreatedTimestamp { get; set; }


  }
}
