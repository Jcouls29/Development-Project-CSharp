using System;

namespace Sparcpoint.Exceptions
{
    public class ParameterRequiredException : Exception
    {
        public string ParameterName { get; }
        public ParameterRequiredException(string parameterName) 
            : base(FormatMessage(parameterName))
        {
            ParameterName = parameterName;
        }

        public ParameterRequiredException(string parameterName, Exception innerException) 
            : base(FormatMessage(parameterName), innerException)
        {
            ParameterName = parameterName;
        }

        public ParameterRequiredException(string parameterName, string message)
            : base(FormatMessage(parameterName, message))
        {
            ParameterName = parameterName;
        }

        public ParameterRequiredException(string parameterName, string message, Exception innerException)
            : base(FormatMessage(parameterName, message), innerException)
        {
            ParameterName = parameterName;
        }

        private static string FormatMessage(string parameterName, string message = "")
            => $"Parameter '{parameterName}' is required. {message}".Trim();
    }
}
