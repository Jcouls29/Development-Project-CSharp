using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Interview.Web.Middleware
{
    /// <summary>
    /// EVAL: Global error handling middleware that centralizes exception-to-HTTP-status mapping.
    /// This eliminates repetitive try-catch blocks in every controller action.
    ///
    /// Exception mapping:
    ///   ArgumentException / ArgumentNullException → 400 Bad Request
    ///   KeyNotFoundException                     → 404 Not Found
    ///   All other exceptions                     → 500 Internal Server Error
    ///
    /// In Development mode, the full exception details are included in the response.
    /// In Production mode, only a generic message is returned to avoid leaking internals.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _Next;
        private readonly ILogger<ErrorHandlingMiddleware> _Logger;
        private readonly IHostEnvironment _Environment;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment environment)
        {
            _Next = next;
            _Logger = logger;
            _Environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ArgumentNullException ex => (HttpStatusCode.BadRequest, ex.Message),
                ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
                KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            // EVAL: Log all errors. Warnings for client errors (4xx), Errors for server errors (5xx).
            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _Logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            }
            else
            {
                _Logger.LogWarning("Client error ({StatusCode}): {Message}", (int)statusCode, exception.Message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                Error = message,
                StatusCode = (int)statusCode
            };

            // EVAL: Include stack trace in Development for easier debugging, hide in Production
            if (_Environment.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError)
            {
                response.Detail = exception.ToString();
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Detail { get; set; }
    }
}
