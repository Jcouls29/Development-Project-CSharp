using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Interview.Web.Filters
{
    /// EVAL: Translates validation errors thrown by the service layer into
    /// 400 Bad Request. Covers ArgumentException and its subclasses
    /// (ArgumentNullException, ArgumentOutOfRangeException).
    public sealed class ArgumentExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not ArgumentException ex) return;

            var problem = new ProblemDetails
            {
                Title = "Invalid request.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
            };
            if (!string.IsNullOrEmpty(ex.ParamName))
                problem.Extensions["parameter"] = ex.ParamName;

            context.Result = new BadRequestObjectResult(problem);
            context.ExceptionHandled = true;
        }
    }
}
