using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

//EVAL: I created this first just to be sure I could pull data from the database using Dapper and see it in the browser.

namespace Interview.Web.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
