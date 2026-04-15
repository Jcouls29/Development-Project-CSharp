using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Sparcpoint
{
    internal static class DbParameterCollectionExtensions
    {
        public static DbParameter AddWithValue(this DbParameterCollection parameters, string name, object value)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var parameter = new SqlParameter(name, value ?? DBNull.Value);
            parameters.Add(parameter);
            return parameter;
        }
    }
}
