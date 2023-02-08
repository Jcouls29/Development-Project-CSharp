using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    public class ProductCategory
    {
        [ExplicitKey]
        public int InstanceId { get; set; }

        public int CategoryInstanceId { get; set; }
    }
}