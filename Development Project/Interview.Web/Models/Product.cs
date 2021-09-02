using Interview.Web.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Models
{
    /// <summary>
    /// BaseEntity
    /// </summary>
    public class Product : EntityBase
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// SKU
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// Active
        /// </summary>
        public bool Active { get; set; }
    }
}
