using Sparcpoint.Application.Abstracts;
using Sparcpoint.Core.Entities;
using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Implementations
{
    public class ProductValidationService : IProductValidationService
    {
        public ValidationResponse ProductIsValid(CreateProductRequest request)
        {
            ValidationResponse validationResponse = new ValidationResponse();
            validationResponse.IsValid = true;
            try
            {
                Validate(request.Name, "Name");
                Validate(request.Description, "Description");
                Validate(request.ProductImageUris, "ProductImageUris");
                Validate(request.ValidSkus, "ValidSkus");

            }
            catch (ArgumentNullException argumentNullException)
            {
                validationResponse.IsValid = false;
                validationResponse.InvalidMessage = String.Format("{0} is required", argumentNullException.Message);
            }
            catch (ArgumentException argumentException)
            {
                validationResponse.IsValid = false;
                validationResponse.InvalidMessage = String.Format("{0} is required", argumentException.Message);
            }
            return validationResponse;

        }
        private void Validate(string value, string name)
        {
            PreConditions.ParameterNotNull(value, name);
            PreConditions.StringNotNullOrWhitespace(value, name);

        }
    }

}