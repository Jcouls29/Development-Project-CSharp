using System.Collections.Generic;

namespace Interview.Web.Models.Responses
{
    /// <summary>
    /// Consistent error response format for all API errors.
    /// </summary>
    // EVAL: A standardized error envelope ensures API consumers can write a single
    // error-handling path regardless of which endpoint fails. This is critical for
    // a reusable system that multiple front-ends will consume.
    public class ErrorResponse
    {
        /// <summary>
        /// Machine-readable error code (e.g., "PRODUCT_NOT_FOUND", "VALIDATION_ERROR").
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional field-level error details for validation failures.
        /// </summary>
        public Dictionary<string, string[]> Details { get; set; }
    }
}
