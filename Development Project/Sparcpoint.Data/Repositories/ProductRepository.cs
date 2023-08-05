using Sparcpoint.Data.Entities.Interfaces;
using Sparcpoint.Data.Entities.Stubs;
using Sparcpoint.Data.Implementations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Data.Repositories
{
	public class ProductRepository
	{
		private readonly object _context;
		// This will be where our crud ops goes
		public ProductRepository(object context)
		{
			// context serves as the object for how we setup our database connections and it's profile
			// this is currently EF but and in the web projecct but would be better off being in the sqlserver abstractions i think
			// can do the setup for the database inside that project
			_context = context;
		}

		public async Task<IEntity> GetById(int id)
		{
			// get a specific product by its id
			return new ProductDtoStub().StubOut();
		}

		public async Task<IEnumerable<IEntity>> GetAll()
		{
			// get all products from the database
			var list = new List<ProductDto>
			{
				new ProductDtoStub().StubOut(),
				new ProductDtoStub().StubOut(),
			};

			return list;
		}

		public async Task<IEntity> Create(IEntity product)
		{
			// add a product to the database
			return new ProductDtoStub().StubOut();
		}

		public void Delete(int id)
		{
			// fill in deletion logic here
		}
	}
}
