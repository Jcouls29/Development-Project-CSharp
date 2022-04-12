using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string StockUnitCode { get; set; }

        public string CategoryDescription { get; set; }
    }
}