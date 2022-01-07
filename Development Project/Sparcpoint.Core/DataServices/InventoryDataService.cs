using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
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

                        var transaction = new InventoryTransactions()
                        {
                            TransactionId = Convert.ToInt32(reader["TransactionId"].ToString()),
                            ProductInstanceId = Convert.ToInt32(reader["ProductInstanceId"].ToString()),
                            Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
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
            return 1;
        }

        public async Task<int> GetInventoryByMetadata(KeyValuePair<string, string> metadata)
        {
            return 1;
        }

        public async Task AddNewInventoryTransaction(InventoryTransactions transaction)
        {

        }

        public async Task RollbackInventoryUpdate(int transactionId)
        {

        }
    }
}
