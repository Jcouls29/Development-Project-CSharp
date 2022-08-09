using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Sparcpoint.Models
{
    public class ProductCategories
    {
        [ForeignKey("Product")]
        public Guid InstanceId { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryInstanceId { get; set; }
        public Category Category { get; set; }
        private Products Product { get; set; }
    }
}
