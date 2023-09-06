using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Repositories
{
	public interface IProductsRepo
	{
		Task<List<Products>> GetAllProducts();
	}

	public class ProductsRepo : IProductsRepo
	{
		public ProductsRepo()
		{ 
		
		}

		public async Task<List<Products>> GetAllProducts()
		{
			throw new NotImplementedException();
		}
	}
}
