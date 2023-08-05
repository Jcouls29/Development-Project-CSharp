using Sparcpoint.Data.Entities.Interfaces;
using System;

namespace Sparcpoint.Data.Implementations
{
	public class ProductDto : IProduct
	{
		public int InstanceId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ProductImageUris { get; set; }
		public string ValidSkus { get; set; }
		public DateTime CreatedTimestamp { get; set; }
	}
}
