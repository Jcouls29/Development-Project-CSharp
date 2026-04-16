using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product : BaseClass
    {
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }
    }
}
