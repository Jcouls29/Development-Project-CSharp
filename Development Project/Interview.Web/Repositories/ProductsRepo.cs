using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Interview.Web.Models;
using Sparcpoint;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Repositories
{
	public interface IProductsRepo
	{
		Task<List<Products>> GetAllProducts();
	}

	public class ProductsRepo : IProductsRepo
	{
		public readonly ISqlExecutor SqlExecutor;
		public JsonDataSerializer jsonDataSerializer;

		public ProductsRepo(ISqlExecutor sqlExecutor)
		{
			SqlExecutor = sqlExecutor ?? throw new ParameterRequiredException($"{nameof(ISqlExecutor)}", "DI cannot be null");
		}

		public async Task<List<Products>> GetAllProducts()
		{
			//Stored Proc GetAllProducts
			//$"SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp" 
			//	"FROM [Instances].[Products]";

			var products = new List<Products>();

			await SqlExecutor.ExecuteAsync<List<Products>>((conn, trans) =>
			{
				using SqlCommand command = new SqlCommand("Instances.GetAllProducts");
				command.CommandType = CommandType.StoredProcedure;

				conn.Open();
				var dataReader = command.ExecuteReader();
				var dataTable = new DataTable();
				dataTable.Load(dataReader);
				var jsonProducts = jsonDataSerializer.Serialize(dataTable);

				products = jsonDataSerializer.Deserialize<List<Products>>(jsonProducts);

				return Task.FromResult(products);
			});

			return new List<Products>();
		}
	}
}
