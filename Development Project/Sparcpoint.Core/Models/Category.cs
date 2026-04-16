using System;
using System.Collections.Generic;
using System.Text;
using Sparcpoint.Models.DTOs;

namespace Sparcpoint.Models
{
    public class Category
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedTimestamp { get; private set; }

        public static Category Create(CategoryDto product)
        {
            return new Category
            {
                CreatedTimestamp = DateTime.Now,
                Name = product.Name,    
                Description = product.Description
            };
        }
    }
}
