// EVAL: Global exception handling middleware catches unhandled exceptions
// and returns a consistent ErrorResponse instead of exposing stack traces.
// In production, this prevents leaking internal details to API consumers.
// In development, the DeveloperExceptionPage takes priority (configured in Startup.cs).

using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;

namespace Interview.Web.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions and returns a standardized error response.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                // RV: ArgumentExceptions from PreConditions or validation are client errors (400)
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, "VALIDATION_ERROR", ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // EVAL: SQL error 547 = FK constraint violation. This occurs when a request
                // references a non-existent product ID or category ID. Handled centrally here
                // so every controller benefits without individual try-catch blocks.
                _logger.LogWarning(ex, "Foreign key constraint violation: {Message}", ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, "REFERENCE_NOT_FOUND",
                    "A referenced entity does not exist. Verify all product IDs and category IDs are valid.");
            }
            catch (SqlException ex) when (ex.Number == -2 || ex.Number == 121)
            {
                // EVAL: SQL error -2 = timeout, 121 = semaphore timeout. Both indicate the
                // database is overloaded or a query is too slow.
                _logger.LogError(ex, "Database timeout: {Message}", ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.ServiceUnavailable, "DATABASE_TIMEOUT",
                    "The database operation timed out. Please try again later.");
            }
            catch (SqlException ex) when (ex.Number == 53 || ex.Number == 40)
            {
                // EVAL: SQL error 53 = server not found, 40 = could not open connection.
                // Database is unreachable.
                _logger.LogError(ex, "Database connection failure: {Message}", ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.ServiceUnavailable, "DATABASE_UNAVAILABLE",
                    "The database is currently unavailable. Please try again later.");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Request timeout: {Message}", ex.Message);
                await WriteErrorResponse(context, HttpStatusCode.ServiceUnavailable, "REQUEST_TIMEOUT",
                    "The request timed out. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

                // EVAL: In production, do NOT expose exception details.
                // In development, include the message for debugging convenience.
                var message = _environment.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred. Please try again later.";

                await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", message);
            }
        }

        private static async Task WriteErrorResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            string code,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                Code = code,
                Message = message
            };

            var json = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(json);
        }
    }
}
