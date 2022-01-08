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
    //EVAL: with more time I'd want to refactor this to use the ISQLExecutor and pull out repeated code to lesson repition
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

        public async Task<int> CreateCategoryAsync(Category newCategory)
        {
            int createdId;

            string commandText = "INSERT [Instances].[Categories] (Name, Description, CreatedTimestamp) OUTPUT Inserted.InstanceId VALUES (@Name, @Description, @CreatedTimestamp)";

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
                    createdId = (int)command.ExecuteScalar();

                    conn.Close();
                }
            }

            return createdId;
        }

        public async Task AddAttributeToCategory(int categoryId, KeyValuePair<string, string> attribute)
        {
            string commandText = "INSERT [Instances].[CategoryAttributes] ([InstanceId], [Key], [Value]) VALUES (@InstanceId, @Key, @Value)";

            SqlParameter parameterCategoryId = new SqlParameter("@InstanceId", categoryId);
            SqlParameter parameterKey = new SqlParameter("@Key", attribute.Key);
            SqlParameter parameterValue = new SqlParameter("@Value", attribute.Value);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterCategoryId);
                    command.Parameters.Add(parameterKey);
                    command.Parameters.Add(parameterValue);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public async Task AddCategoryToCategory(int categoryId, int parentCategoryId)
        {
            string commandText = "INSERT [Instances].[CategoryCategories] (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryInstanceId)";

            SqlParameter parameterCategoryId = new SqlParameter("@InstanceId", categoryId);
            SqlParameter parameterParentCategoryId = new SqlParameter("@CategoryInstanceId", parentCategoryId);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterCategoryId);
                    command.Parameters.Add(parameterParentCategoryId);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }
}
