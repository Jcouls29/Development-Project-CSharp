using Dal.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Dal.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Dal
{
    public class ProductRepository : IProductRepository
    {
        private ISqlExecutor _sqlExecutor;
        private IConfiguration _configuration;
        public ProductRepository(ISqlExecutor sqlExecutor, IConfiguration configuration)
        {
            _sqlExecutor = sqlExecutor;
            _configuration = configuration;
        }

        public List<Products> SearchProductsByCategory(string searchCriteria)
        {
            string query = $"select * from [Instances].[SearchProductsByCategory](@categoryName)";
            IDataReader reader = null;
            List<Products> products = new List<Products>();

            var productReader = _sqlExecutor.Execute<List<Products>>((connection, idbTransaction) =>
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "categoryName",
                        DbType = DbType.String,
                        Value = searchCriteria
                    });
                    command.Transaction = idbTransaction;

                    reader = command.ExecuteReader();
                    if (!reader.IsClosed)
                    {
                        while (reader.Read())
                        {
                            products.Add(new Products
                            {
                                InstanceId = int.Parse(reader["InstanceId"].ToString()),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                ProductImageUris = reader["ProductImageUris"].ToString(),
                                ValidSkus = reader["ValidSkus"].ToString(),
                                CreateTimestamp = DateTime.Parse(reader["CreatedTimestamp"].ToString())
                            });
                        }
                    }
                    reader.Close();
                    reader = null;
                    return products;
                }

                catch (Exception ex)
                {
                    idbTransaction.Rollback();
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    idbTransaction.Rollback();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
                return null;
            });

            return products;
        }
        public List<Products> GetProducts()
        {
            string getProductsQuery = "select InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp from instances.Products";
            IDataReader reader = null;
            List<Products> products = new List<Products>();


            var finalProducts = _sqlExecutor.Execute<List<Products>>((connection, idbTransaction) =>
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = getProductsQuery;

                    command.Transaction = idbTransaction;
                    reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        products.Add(new Products
                        {
                            InstanceId = int.Parse(reader["InstanceId"].ToString()),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            ProductImageUris = reader["ProductImageUris"].ToString(),
                            ValidSkus = reader["ValidSkus"].ToString(),
                            CreateTimestamp = DateTime.Parse(reader["CreatedTimestamp"].ToString())
                        });
                    }
                    reader.Close();
                    reader = null;
                    return products;
                }
                catch (Exception ex)
                {
                    idbTransaction.Rollback();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }

                }

                return null;
            });

            return finalProducts;
        }


        public void UpdateProductCategories(List<int> categoriesId, int productId)
        {
            var updateCategoryProcedureName = "[instances].[CreateProductCategories]";

            if (categoriesId != null && categoriesId.Count > 0)
            {
                foreach (var categoryId in categoriesId)
                {


                    _sqlExecutor.Execute((connection, idbTransaction) =>
                    {
                        try
                        {
                            var command = connection.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = updateCategoryProcedureName;
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "InstanceId",
                                DbType = DbType.Int32,
                                Value = productId
                            });
                            command.Parameters.Add(
                                new SqlParameter
                                {
                                    ParameterName = "CategoryInstanceId",
                                    DbType = DbType.Int32,
                                    Value = categoryId
                                }
                              );
                            command.Transaction = idbTransaction;
                            return command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            idbTransaction.Rollback();
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                        return -1;
                    });

                }
            }
        }

        public List<int> AddCategories(Products product)
        {
            var categoryProcedureName = "[instances].[CreateCategoryIfNotExists]";

            var categories = product.CategoryName.Split(',');
            List<int> categoriesId = new List<int>();

            if (categories != null && categories.Length > 0)
            {
                foreach (var category in categories)
                {
                    var categoryId = _sqlExecutor.Execute((connection, idbTransaction) =>
                    {
                        try
                        {
                            var command = connection.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = categoryProcedureName;
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "name",
                                DbType = DbType.String,
                                Value = category
                            });
                            command.Parameters.Add(
                                new SqlParameter
                                {
                                    ParameterName = "description",
                                    DbType = DbType.String,
                                    Value = product?.Description
                                }
                              );
                            command.Transaction = idbTransaction;
                            return command.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {
                            idbTransaction.Rollback();  
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }

                        return null;
                    });

                    categoriesId.Add(int.Parse(categoryId.ToString()));

                }

                return categoriesId;
            }
            return null;
        }

        public int AddProduct(Products product)
        {
            var cs = _configuration.GetConnectionString("InventoryDatabase");
            string productProcedureName = "[instances].[CreateProductIfNotExists]";


            var productId = _sqlExecutor.Execute((connection, idbTransaction) =>
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = productProcedureName;
                    command.Transaction = idbTransaction;

                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "name",
                        DbType = DbType.String,
                        Value = product.Name
                    });
                    command.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "description",
                            DbType = DbType.String,
                            Value = product?.Description
                        }
                      );
                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "productImageUris",
                        DbType = DbType.String,
                        Value = product.ProductImageUris?.ToString()
                    });

                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "validSkus",
                        DbType = DbType.String,
                        Value = product.ValidSkus?.ToString()
                    });


                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    idbTransaction.Rollback();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }

                return -1;
            });

            return int.Parse(productId.ToString());

        }

    }

}
