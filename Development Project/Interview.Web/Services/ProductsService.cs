using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace Interview.Web.Services
{
	public interface IProductSerivce 
	{
		Task<List<Products>> GetAllProducts();
	}

	public class ProductsService : IProductSerivce
	{
		public IProductsRepo ProductRepo;
		public ILogger<ProductsService> Logger;

		public ProductsService(ILogger<ProductsService> logger, IProductsRepo productRepo)
		{
			Logger = logger ?? throw new ArgumentException($"{nameof(ILogger<ProductsService>)} DI cannot be null");
			ProductRepo = productRepo ?? throw new ArgumentException($"{nameof(IProductsRepo)} DI cannot be null");
		}

		public async Task<List<Products>> GetAllProducts()
		{
			var products = new List<Products>();

			try
			{
				products = await ProductRepo.GetAllProducts();
			}
			catch (Exception ex)
			{
				//log error
			}
			finally 
			{
				//log count & any other important data
			}

			return products;
		}
	}
}
