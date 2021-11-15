using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Dtos
{
    public class InventoryDto
    {

        public ProductDto Product { get; set; }
        public  int Quantity { get; set; }

        public DateTime? CompletedOn { get; set; }

        
    }
}
