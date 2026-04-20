using Interview.Web.Contracts;
using Sparcpoint.Domain;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Interview.Web.Services
{
    /** QueryProductInterpreter is a class that implements the IQueryInterpreter interface for the Product entity. 
     * It takes a query string and converts it into a SqlServerQueryProvider that can be used to filter products based on the specified criteria.
     * */
    public class QueryProductInterpreter : IQueryInterpreter<Product>
    {
        //filter=Name:Test 1,Color:Azul,Largo:10CM
        public SqlServerQueryProvider Interpret(string query)
        {
            var queryProvider = new SqlServerQueryProvider();
            queryProvider.SetTargetTableAlias("p");
            var filters = query.Split(',');
            var num = 1;
            foreach (var f in filters)
            {
                var parts = f.Split(':');
                var key = parts[0];
                var value = parts[1];
                if (key.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    queryProvider.Where($"Name LIKE '%' + @name + '%'");
                    queryProvider.AddParameter("@name", value);
                }
                else if (key.Equals("Description", StringComparison.OrdinalIgnoreCase))
                {
                    queryProvider.Where($"Description LIKE '%' + @description + '%'");
                    queryProvider.AddParameter("@description", value);
                }
                else if (key.Equals("ValidSkus", StringComparison.OrdinalIgnoreCase))
                {
                    queryProvider.Where($"ValidSkus LIKE '%' + @validSkus + '%'");
                    queryProvider.AddParameter("@validSkus", value);
                }
                else
                {
                    queryProvider.Where($"EXISTS (SELECT 1 FROM Instances.ProductAttributes pa WHERE pa.InstanceId = p.InstanceId AND pa.[Key] = @key{num} AND pa.Value like '%' + @value{num} + '%')");
                    queryProvider.AddParameter($"@key{num}", key);
                    queryProvider.AddParameter($"@value{num}", value);
                    num++;
                }
            }

            return queryProvider;
        }
    }
}
