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
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedTimestamp { get; }
    }
}
