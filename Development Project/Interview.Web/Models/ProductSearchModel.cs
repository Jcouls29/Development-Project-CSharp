namespace Interview.Web.Models
{
     //EVAL: This will be used to control how we will search items and what query params we can pass in
     // like the CreateProductModel this can be extended if there are more params added in the future
     public class ProductSearchModel
     {
          public string SKU { get; set; }
          public string Name { get; set; }

     }
}
