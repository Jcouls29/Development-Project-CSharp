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

        public static void StringMaxLength(string value, string parameterName, int maxLength)
        {
            if (value != null && value.Length > maxLength)
                throw new ArgumentException($"{parameterName} must not exceed {maxLength} characters.", parameterName);
        }

        public static void IntGreaterThanZero(int value, string parameterName)
        {
            if (value <= 0)
                throw new ArgumentException($"{parameterName} must be greater than zero.", parameterName);
        }
    }
}
