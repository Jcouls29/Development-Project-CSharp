using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    [Table("[Instances].[Categories]")]
    public class Category
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        [Computed]
        public List<CategoryAttribute> CategoryAttributes { get; set; }
        [Computed]
        public List<CategoryOfCategory> Categories { get; set; }

    }
}