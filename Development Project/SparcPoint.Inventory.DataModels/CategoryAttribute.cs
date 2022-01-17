using System;
using System.Collections.Generic;
using System.Text;

namespace SparcPoint.Inventory.DataModels
{
    public class CategoryAttribute :IAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
