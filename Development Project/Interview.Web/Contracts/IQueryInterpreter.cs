using Microsoft.AspNetCore.Http;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Interview.Web.Contracts
{
    /** IQueryInterpreter is an interface that defines a contract for interpreting a query string and converting it into a function that can be used to filter entities.
     */
    public interface IQueryInterpreter<T>
    {
        SqlServerQueryProvider Interpret(string query);
    }
}
