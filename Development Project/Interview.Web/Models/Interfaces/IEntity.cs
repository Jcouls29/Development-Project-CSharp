using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Models.Interfaces
{
    /// <summary>
    /// IEntity
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
