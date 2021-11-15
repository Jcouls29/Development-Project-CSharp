using Domain.Entity;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entitys
{
    public class Inventory : BaseEntity, IInventory
    {


        public Inventory(Product product, int quantity, DateTime? completedDateTime)
        {
            this.Product = product;
            this.Quantity = quantity;
           
            this.CompletedDateTime = completedDateTime;
        }
        public Product Product { get; set; }
        public int Quantity { get; set; }


        public DateTime? CompletedDateTime { get; set; }

        public DateTime CreatedTimestamp { get; }
    }
}
