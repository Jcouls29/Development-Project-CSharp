using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Services
{
    public interface IValidationService
    {
        ValidationResponse CategoryIsValid(CreateCategoryRequest request);
        ValidationResponse QuantityIsValid(int quantity);
        ValidationResponse ProductIsValid(CreateProductRequest req);
        ValidationResponse SearchIsValid(ProductSearchRequest req);
    }
}
