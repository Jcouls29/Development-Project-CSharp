using System;
using System.Collections.Generic;
using System.Linq;

namespace Sparcpoint
{
    public class ParameterRequiredException : Exception
    {
        public List<string> ParameterNames { get; }
        public ParameterRequiredException(List<string> parameterNames) 
            : base(FormatMessage(parameterNames))
        {
            ParameterNames = parameterNames;
        }

        public ParameterRequiredException(List<string> parameterNames, Exception innerException) 
            : base(FormatMessage(parameterNames), innerException)
        {
            ParameterNames = parameterNames;
        }

        public ParameterRequiredException(List<string> parameterNames, string message)
            : base(FormatMessage(parameterNames, message))
        {
            ParameterNames = parameterNames;
        }

        public ParameterRequiredException(List<string> parameterNames, string message, Exception innerException)
            : base(FormatMessage(parameterNames, message), innerException)
        {
            ParameterNames = parameterNames;
        }

        private static string FormatMessage(List<string> parameterNames, string message = "")
        {
            string missingParameterNames = String.Join(", ", parameterNames);
            return $"{missingParameterNames} required. {message}".Trim();
        }
    }
}
