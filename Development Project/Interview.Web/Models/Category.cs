using Interview.Web.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Models
{
    public class Category: EntityBase
    {
        public Guid ProductId { get; set; }
        public HashSet<String> Name { get; set; }
    }
}
