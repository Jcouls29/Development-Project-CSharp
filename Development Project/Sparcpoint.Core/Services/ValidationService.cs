using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparcpoint.Services
{
    public class ValidationService: IValidationService
    {
        public ValidationResponse CategoryIsValid(CreateCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Name is required",
                    IsValid = false
                };
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Description is required",
                    IsValid = false
                };
            }

            return new ValidationResponse()
            {
                InvalidMessage = null,
                IsValid = true
            };
        }

        public ValidationResponse QuantityIsValid(int quantity)
        {
            if (quantity < 0 || quantity == null)
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Quantity of inventory must be positive",
                    IsValid = false
                };
            }

            return new ValidationResponse()
            {
                InvalidMessage = null,
                IsValid = true
            };
        }

        public ValidationResponse ProductIsValid(CreateProductRequest req)
        {
            if (string.IsNullOrEmpty(req.Name))
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Name is required",
                    IsValid = false
                };
            }

            if (string.IsNullOrEmpty(req.Description))
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Description is required",
                    IsValid = false
                };
            }

            if (req.ProductImageUris == null || !req.ProductImageUris.Any())
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "At least one image uri is required",
                    IsValid = false
                };
            }

            if (req.ValidSkus == null || !req.ValidSkus.Any())
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "At least one sku is required",
                    IsValid = false
                };
            }

            //EVAL: would get business requirements for valid sku and add that here

            return new ValidationResponse()
            {
                InvalidMessage = null,
                IsValid = true
            };
        }

        public ValidationResponse SearchIsValid(ProductSearchRequest req)
        {
            var validSearchFields = new List<string>() { "metadata", "categories", "name", "description", "all" };
            
            if (string.IsNullOrEmpty(req.Keyword))
            {
                return new ValidationResponse()
                {
                    InvalidMessage = "Keyword is required",
                    IsValid = false
                };
            }

            if(req.SearchBy != null)
            {
                foreach (var searchBy in req.SearchBy)
                {

                    if (!validSearchFields.Contains(searchBy.ToLower()))
                    {
                        return new ValidationResponse()
                        {
                            InvalidMessage = "The search by field is not a valid option",
                            IsValid = false
                        };
                    }
                }
            }

            return new ValidationResponse()
            {
                InvalidMessage = null,
                IsValid = true
            };
        }
    }
}
