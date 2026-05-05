using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Contexts;

public class ProductContext
{
    public static ProductModel CreateProduct(CreateProductDto product)
    {
        var sql = @"INSERT INTO Products
                        ([Description], [Name], ProductImageUris, ValidSkus)
                    VALUES
                        (@Description, @Name, @ProductImageUris, @ValidSkus)
                    ";
        // TODO: Execute SQL ~ var newProd = ...
        return new ProductModel();
    }
    
    public static List<ProductModel> GetAllProducts()
    {
        var sql = "SELECT * FROM Products";
        // TODO: Execute SQL ~ var products = ...
        return new List<ProductModel>();
    }
}