using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Instance.Entities
{
    [Table("[Instances].[ProductAttributes]")]
    public class ProductAttribute
    {
        [ExplicitKey]
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
