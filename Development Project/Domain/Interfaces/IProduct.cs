using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface IProduct
    {
        string ProductName { get; set; }
        string ProductDescription { get; set; }

        string StockUnitCode { get; set; }
        DateTime CreatedTimestamp { get; }
    }
}