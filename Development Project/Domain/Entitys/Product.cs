using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Product : BaseEntity, IProduct
    {

        public Product(string name, string desc, string sku, Category category)
        {

            this.ProductName = productame;
            this.ProductDescription = desc;
            this.StockUnitCode = sku;
            this.ProductCategory = productcategory;
        }


        public ProductCategory ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public string StockUnitCode { get; set; }

        public DateTime CreatedTimestamp { get; }


    }
}