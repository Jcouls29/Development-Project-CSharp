using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Product : BaseEntity, IProduct
    {

        public Product(string name, string desc, string sku , Category category)
        {

            this.Name = name;
            this.Description = desc;
            this.StockKeepUnitCode = sku;
            this.Category = category;
        }


        public Category Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string StockKeepUnitCode { get; set; }

        public DateTime CreatedTimestamp { get; }


    }
}
