using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IInventory
    {
        Product Product { get; set; }
        int Quantity { get; set; }

        DateTime? CompletedDateTime { get; set; }

        DateTime CreatedTimestamp { get; }
    }
}