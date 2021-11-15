using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface IProduct
    {
        string Name { get; set; }
        string Description { get; set; }

        string StockKeepUnitCode { get; set; }
        DateTime CreatedTimestamp { get; }
    }
}
