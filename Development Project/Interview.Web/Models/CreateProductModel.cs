namespace Interview.Web.Models
{
     //EVAL: Used for fine grained control over what is passed into the post endpoint's body
     //In the future if we extend the Product model this will make it easier to add more attributes
     public class CreateProductModel
     {
          public string Name { get; set; }
          public string Description { get; set; }
          public string ProductImageUris { get; set; }
          public string ValidSkus { get; set; }
     }
}
