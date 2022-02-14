using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Instance.Entities
{
    [Table("[Instances].[Products]")]
    public class Product 
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        [Computed]
        public List<ProductAttribute> ProductAttributes { get; set; }
        [Computed]
        public List<ProductCategory> Categories { get; set; }
        [Computed]
        public int Inventory { get; set; }



    }
}
