using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Interview.Web.Entities
{
    public class Products
    {
		public int InstanceId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ProductImageUris { get; set; }
		public string ValidSkus { get; set; }
		public DateTime CreatedTimestamp { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public ICollection<ProductAttributes> ProductAttributes { get; set; }
		//public ICollection<ProductCategories> ProductCategories { get; set; }
	}
}
