using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sparcpoint.Models
{
    public class Inventory
    {
        //[ForeignKey("Product")]
        //public Guid InstanceId { get; set; }
        [Key]
        public Guid InventoryId { get; set; }
        //public Products Product { get; set; }
    }
}
