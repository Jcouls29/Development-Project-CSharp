using Interview.Web.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Models
{
    public class EntityBase:IEntity
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
