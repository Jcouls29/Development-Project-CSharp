using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Interview.Web.Core.DomainModel
{
    public class ProdusctRequest
    {
		public int InstanceId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ProductImageUris { get; set; }
		public string ValidSkus { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public List<ProductAttributeRequest> ProductAttributes { get; set; }
	}
}
