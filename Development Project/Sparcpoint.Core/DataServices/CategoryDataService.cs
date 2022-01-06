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
    public class CategoryDataService : ICategoryDataService
    {
        private string _dbConn;

        public CategoryDataService(string dbConn) => _dbConn = dbConn;

        public async Task<List<Category>> GetCategories()
        {
            var categoryList = new List<Category>();

            string queryString = "SELECT * FROM [Instances].[Categories];";
            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timestamp = DateTime.Parse(reader["CreatedTimestamp"].ToString());

                        var cateogry = new Category()
                        {
                            InstanceId = Convert.ToInt32(reader["InstanceId"].ToString()),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedTimestamp = timestamp
                        };

                        categoryList.Add(cateogry);
                    }
                }

                conn.Close();
            }

            return categoryList;
        }

        public async Task CreateCategoryAsync(Category newCategory)
        {
            var categoryList = new List<Category>();

            String commandText = "INSERT [Instances].[Categories] (Name, Description, CreatedTimestamp) VALUES (@Name, @Description, @CreatedTimestamp)";

            SqlParameter parameterName = new SqlParameter("@Name", newCategory.Name);
            SqlParameter parameterDescription = new SqlParameter("@Description", newCategory.Description);
            SqlParameter parameterCreatedOn = new SqlParameter("@CreatedTimestamp", newCategory.CreatedTimestamp);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterName);
                    command.Parameters.Add(parameterDescription);
                    command.Parameters.Add(parameterCreatedOn);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }
}
