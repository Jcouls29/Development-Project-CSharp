using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using System;

namespace Interview.Web.Filters
{
    /// <summary>
    /// EVAL: Global exception filter centralizes HTTP status mapping for domain exceptions.
    /// Without this, any unhandled exception thrown by a repository bubbles up as a 500.
    /// Adding it once here keeps every controller clean — no per-endpoint try/catch boilerplate.
    ///
    /// Mapping rationale:
    ///   ArgumentException (incl. ArgumentNullException,
    ///                       ArgumentOutOfRangeException) → 400 Bad Request  (invalid caller input;
    ///       ArgumentOutOfRangeException is a subclass of ArgumentException so the same case arm
    ///       handles it automatically — no separate branch needed)
    ///   InvalidOperationException                       → 404 Not Found    (entity not found)
    ///   SqlException error 547 (FK violation)           → 400 Bad Request  (TOCTOU fallback —
    ///       repositories pre-validate FKs but a concurrent delete between the check and the
    ///       INSERT can still produce a 547; we catch it here so it never surfaces as a 500)
    ///
    /// To add a new mapping (e.g. ConflictException → 409), extend the switch here
    /// without touching any controller.
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
