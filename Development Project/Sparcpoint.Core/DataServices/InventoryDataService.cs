using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    //EVAL: with more time I'd want to refactor this to use the ISQLExecutor and pull out repeated code to lesson repition
    public class InventoryDataService: IInventoryDataService
    {
        private readonly string _connString;

        public InventoryDataService(string connString)
        {
            _connString = connString;
        }


        public async Task<List<InventoryTransactions>> GetAllInventoryTransactions()
        {
            var transactionList = new List<InventoryTransactions>();

            string queryString = "SELECT * FROM [Transactions].[InventoryTransactions];";
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var startedTimestamp = DateTime.Parse(reader["StartedTimestamp"].ToString());
                        var completedTimestamp = DateTime.Parse(reader["CompletedTimestamp"].ToString());
                        var quantity = Convert.ToDecimal(reader["Quantity"].ToString());

                        var transaction = new InventoryTransactions()
                        {
                            TransactionId = Convert.ToInt32(reader["TransactionId"].ToString()),
                            ProductInstanceId = Convert.ToInt32(reader["ProductInstanceId"].ToString()),
                            Quantity = decimal.ToInt32(quantity),
                            StartedTimestamp = startedTimestamp,
                            CompletedTimestamp = completedTimestamp,
                            TypeCategory = reader["TypeCategory"].ToString()
                        };

                        transactionList.Add(transaction);
                    }
                }

                conn.Close();
            }

            return transactionList;
        }

        public async Task<int> GetInventoryForProduct(int productId)
        {
            int productQuantity = 0;

            string queryString = "SELECT TOP 1 Quantity FROM [Transactions].[InventoryTransactions] WHERE ProductInstanceId = @ProductInstanceId ORDER BY CompletedTimestamp DESC;";
            SqlParameter parameterProductId = new SqlParameter("@ProductInstanceId", productId);

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.Add(parameterProductId);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var quantity = Convert.ToDecimal(reader["Quantity"].ToString());
                        productQuantity = decimal.ToInt32(quantity);
                    }
                }

                conn.Close();
            }

            return productQuantity;
        }

        
        public async Task<int> AddNewInventoryTransaction(InventoryTransactions transaction)
        {
            int createdId;

            string commandText = "INSERT [Transactions].[InventoryTransactions] (ProductInstanceId, Quantity, StartedTimestamp, CompletedTimestamp, TypeCategory) OUTPUT Inserted.TransactionId VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

            SqlParameter parameterProductId = new SqlParameter("@ProductInstanceId", transaction.ProductInstanceId);
            SqlParameter parameterQuantity = new SqlParameter("@Quantity", transaction.Quantity);
            SqlParameter parameterStarted = new SqlParameter("@StartedTimestamp", transaction.StartedTimestamp);
            SqlParameter parameterCompleted = new SqlParameter("@CompletedTimestamp", transaction.CompletedTimestamp);
            SqlParameter parameterCategory = new SqlParameter("@TypeCategory", transaction.TypeCategory);

            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    cmd.Parameters.Add(parameterProductId);
                    cmd.Parameters.Add(parameterQuantity);
                    cmd.Parameters.Add(parameterStarted);
                    cmd.Parameters.Add(parameterCompleted);
                    cmd.Parameters.Add(parameterCategory);

                    conn.Open();
                    createdId = (int)cmd.ExecuteScalar();

                    conn.Close();
                }
            }

            return createdId;
        }

        public async Task RollbackInventoryUpdate(int transactionId)
        {
            string commandText = "DELETE FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @TransactionId;";
            SqlParameter parametertransId = new SqlParameter("@TransactionId", transactionId);


            using (SqlConnection conn = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    cmd.Parameters.Add(parametertransId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }
}
