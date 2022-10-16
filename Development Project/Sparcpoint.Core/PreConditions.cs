using System;

namespace Sparcpoint
{
    public static class PreConditions
    {
        public static void ParameterNotNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void StringNotNullOrWhitespace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        public static void StringLengthDoesNotExceed(string value, int length, string parameterName)
        {
            if (value.Length > length)
                throw new ArgumentException($"{parameterName} max length ({length}) exceeded.", parameterName);
        }
    }
}
