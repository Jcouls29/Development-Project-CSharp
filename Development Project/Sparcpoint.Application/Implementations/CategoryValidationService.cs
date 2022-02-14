using Sparcpoint.Application.Abstracts;
using Sparcpoint.Domain.Requestes;
using Sparcpoint.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Implementations
{
    public class CategoryValidationService : ICategoryValidationService
    {
        public ValidationResponse CategoryIsValid(CreateCategoryRequest request)
        {
            ValidationResponse validationResponse = new ValidationResponse();
            validationResponse.IsValid = true;
            try
            {
                PreConditions.ParameterNotNull(request.Name, "Name");
                PreConditions.ParameterNotNull(request.Description, "Description");
                PreConditions.StringNotNullOrWhitespace(request.Name, "Name");
                PreConditions.StringNotNullOrWhitespace(request.Description, "Description");
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
    }
}
