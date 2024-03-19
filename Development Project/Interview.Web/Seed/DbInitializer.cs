using Interview.Web.Enums;
using Sparkpoint.Data;
using System;
using System.Linq;

namespace Interview.Web.Seed
{
    public static class DbInitializer
    {
        public static void Initialize(InventoryContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }
            // seed some categories
            var categories = new Category[]{
                new Category
                {
                    Name = "Electronics",
                    Description = "Electronics encompass a wide range of devices that operate through electric circuits. These products often enhance communication, entertainment, and productivity"
                },
                new Category
                {
                    Name = "Smartphones",
                    Description = "Mobile devices with advanced computing capabilities."
                },

            };
            foreach (var c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            // link top level category to low level category
            int topLevelCategoryId = context.Categories.First(c => c.Name == "Electronics").InstanceId;
            int lowLevelCategoryId = context.Categories.First(c => c.Name == "Smartphones").InstanceId;

            var categoryLink = new CategoryCategory
            {
                CategoryInstanceId = topLevelCategoryId,
                InstanceId = lowLevelCategoryId,
            };
            context.CategoryCategories.Add(categoryLink);
            context.SaveChanges();


            var productCategory = context.Categories.FirstOrDefault();
            if (productCategory != null)
            {
                Console.WriteLine(productCategory.Name);
            }
            var products = new Product[]
            {
                new Product{Name="IPhone 15 Pro Max",Description="The latest iPhone model with advanced features.", ProductImageUris="iphone13.jpg", ValidSkus="IPH13-128GB-BLK, IPH13-256GB-BLU, IPH13-512GB-RED", }};
            // TODO: add more products
            foreach (var p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();


            // Product is added. we can link to category now. 

            var product = context.Products.FirstOrDefault();
            // shouldn't happen, but just in case. 
            if (product != null)
            {
                context.ProductCategories.Add(new ProductCategory
                {
                    InstanceId = product.InstanceId,
                    CategoryInstanceId = lowLevelCategoryId
                });
                context.SaveChanges();

                // since I am working with queried product, let's update transaction table as well. 
                context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductInstanceId = product.InstanceId,
                    Quantity = 1,
                    TypeCategory = TransactionTypesEnum.Create.ToString(),
                });
                context.SaveChanges();

                // I can create product attribute

                context.ProductAttributes.Add(new ProductAttribute
                {
                    InstanceId = product.InstanceId,
                    Key = AttributeTypesEnum.Color.ToString(),
                    Value = "white" // I think rgb representation would be great here. 
                    // maybe different table with colors? so UI will use rgb colors, but customers will see actual description of the common colors. 
                });
                context.SaveChanges();
            }

            context.SaveChanges();
        }
    }
}
