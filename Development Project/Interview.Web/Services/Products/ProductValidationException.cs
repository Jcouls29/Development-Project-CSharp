using System;

namespace Interview.Web.Services.Products
{
    public class ProductValidationException : Exception
    {
        public ProductValidationException(string message) : base(message)
        {
        }
    }
}
