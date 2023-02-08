using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    [Table("[Instances].[CategoryAttributes]")]
    public class CategoryAttribute
    {
        [ExplicitKey]
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}