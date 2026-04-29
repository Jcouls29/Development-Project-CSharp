using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System;

namespace Interview.Web.Filters
{
    /// <summary>
    /// EVAL: maps exceptions to HTTP codes in one place so controllers don't need try/catch blocks
    /// everywhere. ArgumentException (including Null and OutOfRange subclasses) becomes 400,
    /// InvalidOperationException becomes 404, and SQL FK violations (error 547) also get caught
    /// as 400 as a TOCTOU fallback - repos pre-validate FKs but a concurrent delete between
    /// check and INSERT can still produce a 547. To add a new mapping just extend the switch here.
    /// </summary>
    public class ApiExceptionFilter : IActionFilter
    {
        // SQL Server error number for foreign key constraint violation.
        private const int SqlForeignKeyViolation = 547;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
                return;

            switch (context.Exception)
            {
                case ArgumentException ex:
                    context.Result = new BadRequestObjectResult(new { error = ex.Message });
                    context.ExceptionHandled = true;
                    break;

                case InvalidOperationException ex:
                    context.Result = new NotFoundObjectResult(new { error = ex.Message });
                    context.ExceptionHandled = true;
                    break;

                case SqlException ex when ex.Number == SqlForeignKeyViolation:
                    context.Result = new BadRequestObjectResult(
                        new { error = "A referenced entity does not exist or has been removed." });
                    context.ExceptionHandled = true;
                    break;
            }
        }
    }
}
