using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Interview.Web.Infrastructure
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiValidationException validationException)
            {
                context.Result = BuildResult(400, "Validation failed", validationException.Message);
                context.ExceptionHandled = true;
                return;
            }

            if (context.Exception is ResourceNotFoundException notFoundException)
            {
                context.Result = BuildResult(404, "Resource not found", notFoundException.Message);
                context.ExceptionHandled = true;
                return;
            }

            _logger.LogError(context.Exception, "Unhandled API exception.");
            var detail = _hostEnvironment.IsDevelopment()
                ? BuildDevelopmentDetail(context.Exception)
                : "An unexpected error occurred while processing the request.";

            context.Result = BuildResult(500, "Unexpected error", detail);
            context.ExceptionHandled = true;
        }

        private static string BuildDevelopmentDetail(System.Exception exception)
        {
            if (exception.InnerException == null)
                return $"{exception.GetType().Name}: {exception.Message}";

            return $"{exception.GetType().Name}: {exception.Message} | Inner: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}";
        }

        private static ObjectResult BuildResult(int statusCode, string title, string detail)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            })
            {
                StatusCode = statusCode
            };
        }
    }
}
