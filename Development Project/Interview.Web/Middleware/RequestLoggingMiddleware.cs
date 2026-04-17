using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Interview.Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Incoming request {Method} {Path}", context.Request.Method, context.Request.Path);
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                var status = context.Response?.StatusCode;
                _logger.LogInformation("Request {Method} {Path} completed with status {Status} in {Elapsed}ms", context.Request.Method, context.Request.Path, status, sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
