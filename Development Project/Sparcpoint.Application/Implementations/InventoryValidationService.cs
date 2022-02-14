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
    public class InventoryValidationService : IInventoryValidationService
    {
        public ValidationResponse InventoryIsValid(InventoryTransactionRequest request)
        {
            ValidationResponse validationResponse = new ValidationResponse();
            validationResponse.IsValid = true;
            if (request.ProductInstanceId == 0)
            {
                validationResponse.IsValid = false;
                validationResponse.InvalidMessage = "ProductInstanceId is invalid"; 
            }
            if (request.Quantity == 0)
            {
                validationResponse.IsValid = false;
                validationResponse.InvalidMessage = "Quantity is invalid";
            }
            return validationResponse;

        }
    }
}
