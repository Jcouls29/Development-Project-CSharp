using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Category : BaseEntity, ICategory
    {
        public Category()
        {

        }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public DateTime CreatedTimestamp { get; }
    }
}