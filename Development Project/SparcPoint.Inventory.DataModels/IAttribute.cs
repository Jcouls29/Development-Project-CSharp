using System;
using System.Collections.Generic;
using System.Text;

namespace SparcPoint.Inventory.DataModels
{
    public interface IAttribute
    {
        int InstanceId { get; set; }
        string Key { get; set; }
        string Value { get; set; }
    }
}
