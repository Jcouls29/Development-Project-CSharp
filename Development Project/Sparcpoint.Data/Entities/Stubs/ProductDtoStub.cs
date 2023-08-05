using Sparcpoint.Data.Implementations;

namespace Sparcpoint.Data.Entities.Stubs
{
	public class ProductDtoStub
	{
		public ProductDtoStub() { }

		public ProductDto StubOut()
		{
			return new ProductDto()
			{
				Name = "TestName_1",
				Description = "TestDesc_1",
				ProductImageUris = "TestUri_1",
				ValidSkus = "TestSku_1",
				CreatedTimestamp = System.DateTime.Now
			};
		}
	}
}
