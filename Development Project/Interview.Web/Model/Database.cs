using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Model
{
    public class Database
    {
        

        public async static Task<string> AddProduct(Products Product)
        {
            string Stringofdata = "Failed";
            try
            {/* Hit a SP in database that would Add Product*/
               // using (IDbTransaction Transaction = con.BeginTransaction())
                {
                    try
                    {
                        await using (con)
                        {
                            //using (SqlCommand cmd = new SqlCommand("AddProduct", con))
                            {
                                //cmd.CommandType = CommandType.StoredProcedure;
                                // Our Parameters
                                //sqlComm.Parameters.AddWithValue("@Name", Product.Name);
                                //cmd.Parameters["@Stringofdata"].Direction = ParameterDirection.Output;
                               // con.Open();
                               // cmd.ExecuteNonQuery();
                                Stringofdata = "complete";
                            }

                        }
                       // Transaction.Commit();
                    }
                    catch
                    {
                       // Transaction.Rollback();
                        throw;
                    }
                }
                
                return Stringofdata;

            }
            catch (Exception)
            {
                return Stringofdata;
            }
        }

        

        public async static Task<string> GetProduct(Products Product)
        {
            string Stringofdata = "Failed";
            try
            {   /* Hit a SP in database that would Get Product */
                await using (SqlConnection con = new SqlConnection(Configuration.GetSection("Data").GetSection("ConnectionString").Value))
                {
                    //SqlCommand sqlComm = new SqlCommand("GetProduct", con);
                    //sqlComm.Parameters.AddWithValue("@Name", Product.Name);

                    //sqlComm.CommandType = CommandType.StoredProcedure;
                    //con.Open();
                    //using (var reader = sqlComm.ExecuteReader())
                    {
                        // while (reader.Read())
                        {
                            //Stringofdata = reader["Databack"].ToString();
                            Stringofdata = "complete";
                        }

                    }

                }
                return Stringofdata;

            }
            catch (Exception)
            {
                return Stringofdata;
            }
        }

        public async static Task<string> RemoveProductfromInventory(List<Products> listOfProducts)
        {
            string Stringofdata = "Failed";
            try
            {
                await using (SqlConnection thist = new SqlConnection(configuration.GetSection("Data").GetSection("ConnectionString").Value))
                {
                    /* Hit a SP in database that would Remove Product from Inventory */
                    foreach (var item in listOfProducts)
                    {
                        
                        //SqlCommand sqlComm = new SqlCommand("RemoveProductfromInventory", con);
                        //sqlComm.Parameters.AddWithValue("@Name", item.Name);

                        //sqlComm.CommandType = CommandType.StoredProcedure;
                        //con.Open();
                        //sqlComm.ExecuteReader();
                    }


                    Stringofdata = "complete";

                }
                return Stringofdata;

            }
            catch (Exception)
            {
                return Stringofdata;
            }
        }

        
        public async static Task<List<Products>> searchProductByTags(List<ProductsAttributes> listOfProductsAttributes)
        {
          
            var response = new List<Products>();
            try
            {
                await using (SqlConnection thist = new SqlConnection(configuration.GetSection("Data").GetSection("ConnectionString").Value))
                {
                    /* Hit a SP in database that would search Product ByTags */
                    foreach (var item in listOfProductsAttributes)
                    {

                        SqlCommand sqlComm = new SqlCommand("searchProductByTags", con);
                        sqlComm.Parameters.AddWithValue("@Name", item.Value);

                        sqlComm.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        using (var reader = sqlComm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var Product = new Products();
                                Product.Name = reader["Name"].ToString();
                                Product.Description = reader["Description"].ToString();

                                response.Add(Product);
                            }
                        }
                    } 

                }
                return response;

            }
            catch (Exception)
            {
                return response;
            }
        }

    }
}
