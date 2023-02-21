using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.InventoryService.Common.Extensions
{
    public static class NullableObjectExtensions
    {
        public static T ThrowIfNull<T>(this T parameter, string parameterName) where T : class
        {
            parameterName = parameterName ?? string.Empty;

            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return parameter;
        }
    }
}
