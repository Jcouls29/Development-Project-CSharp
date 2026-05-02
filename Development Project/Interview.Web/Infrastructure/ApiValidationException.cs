using System;

namespace Interview.Web.Infrastructure
{
    public class ApiValidationException : Exception
    {
        public ApiValidationException(string message)
            : base(message)
        {
        }
    }
}
