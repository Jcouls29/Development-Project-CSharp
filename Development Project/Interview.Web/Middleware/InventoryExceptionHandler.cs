using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Middleware
{
    // EVAL: Centralised exception handler maps known exception types to appropriate HTTP status
    // codes and returns RFC 7807 ProblemDetails responses. Keeps controllers free of try/catch
    // blocks and ensures a consistent error shape across all endpoints.
    public class InventoryExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid request"),
                ArgumentException     => (StatusCodes.Status400BadRequest, "Invalid request"),
                _                     => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title  = title,
                // EVAL: exception.Message is safe to expose for ArgumentException — it contains
                // the validation message written by PreConditions or the repository guard clauses.
                // For unexpected exceptions the message may contain sensitive detail; consider
                // logging and returning a generic message in production.
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
