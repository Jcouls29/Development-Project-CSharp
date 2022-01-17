using System;
using System.Collections.Generic;
using System.Text;

namespace SparcPoint.Inventory.DataModels
{
    public class Category
    {
        public Category()
        {
            // Populate Category Attributes
        }

        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<IAttribute> Attributes { get; set; }
        
    }
}
