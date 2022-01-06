using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    public class ProductDataService : IProductDataService
    {
        private string _dbConn;

        public ProductDataService(string dbConn) => _dbConn = dbConn;

        public async Task<List<Product>> GetProducts()
        {
            var productList = new List<Product>();

            string queryString = "SELECT * FROM [Instances].[Products];";
            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timestamp = DateTime.Parse(reader["CreatedTimestamp"].ToString());

                        var product = new Product()
                        {
                            InstanceId = Convert.ToInt32(reader["InstanceId"].ToString()),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            ProductImageUris = reader["ProductImageUris"].ToString(),
                            ValidSkus = reader["ValidSkus"].ToString(),
                            CreatedTimestamp = timestamp
                        };

                        productList.Add(product);
                    }
                }

                conn.Close();
            }

            return productList;
        }

        public async Task CreateProductAsync(Product newProduct)
        {
            var productList = new List<Product>();

            String commandText = "INSERT [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)";

            SqlParameter parameterName = new SqlParameter("@Name", newProduct.Name);
            SqlParameter parameterDescription = new SqlParameter("@Description", newProduct.Description);
            SqlParameter parameterImages = new SqlParameter("@ProductImageUris", newProduct.ProductImageUris);
            SqlParameter parameterSkus = new SqlParameter("@ValidSkus", newProduct.ValidSkus);
            SqlParameter parameterCreatedOn = new SqlParameter("@CreatedTimestamp", newProduct.CreatedTimestamp);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterName);
                    command.Parameters.Add(parameterDescription);
                    command.Parameters.Add(parameterImages);
                    command.Parameters.Add(parameterSkus);
                    command.Parameters.Add(parameterCreatedOn);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        //private Task<List<Product>> GetProductData(IDbConnection connection, IDbTransaction trans)
        //{

        //}

        //private Task CreateNewProduct(IDbConnection connection, IDbTransaction trans, Product product)
        //{

        //    using ()
        //    {
        //    }
        //}
    }
}
