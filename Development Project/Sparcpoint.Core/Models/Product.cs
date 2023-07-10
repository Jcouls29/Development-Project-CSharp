using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
    }
}
