using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models.Products
{
    public class BaseProduct : ProductEntity
    {
        public List<ProductAttributes> Metadata { get; set; }
    }
}
