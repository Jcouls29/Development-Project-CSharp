using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sparcpoint.Inventory.Handler.Commands;
using System.ComponentModel.DataAnnotations;
using Sparcpoint.Inventory.Repository.Interfaces;
using Sparcpoint.InventoryService.Common.Extensions;
using Sparcpoint.Inventory.Core.Requests;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Core.Response;

namespace Sparcpoint.InventoryService.Handler.Commands
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, AddProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AddProductHandler> _logger;

        public AddProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository.ThrowIfNull(nameof(productRepository));
        }

        public async Task<AddProductResponse> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            //EVAL: I usually do validation using FluentValidation _validator.Validate(request).ThrowExceptionIfInvalid();
            AddProductResponse addProductResponse = new AddProductResponse();

            try
            {
                //EVAL: Installed automapper but for now will just map manually...
                var addProductRequest = new AddProductRequest
                {
                    Name= request.Name,
                    Description= request.Description,
                    ProductImageUris= request.ProductImageUris,
                    ValidSkus = request.ValidSkus
                };

                var addCategoryRequests = new List<AddCategoryRequest>();
                foreach (var category in request.Categories)
                {
                    var addCategoryRequest = new AddCategoryRequest
                    {
                        Name = category.Name,
                        Description= category.Description,                        
                    };

                    addCategoryRequests.Add(addCategoryRequest);
                }

                //EVAL: TODO Add Transaction Scope
                var productInstanceId = await _productRepository.AddProductAsync(addProductRequest);
                addProductResponse.InstanceId = productInstanceId;

                request.ProductAttributes.ForEach(productAttribute => productAttribute.InstanceId = productInstanceId);
                addProductResponse.ProductAttributeInstanceIds = await _productRepository.AddProductAttributesAsync(request.ProductAttributes);

                //EVAL: TODO Add Categories and then CategoryAttributes
                /*
                 * var addCategoryAttributesRequest = new List<AddCategoryAttributesRequest>();
                    foreach (var categoryAttribute in category.CategoryAttributes)
                    {
                        addCategoryAttributesRequest.Add(new AddCategoryAttributesRequest
                        {
                            InstanceId = category.InstanceId,
                            Key = categoryAttribute.Key,
                            Value = categoryAttribute.Value,
                        });
                    }
                 */


            }
            catch (Exception e)
            {
                //EVAL: Not properly implemented... therefore commenting for now...
                //_logger.LogError(e, e.Message);
                throw;
            }

            return addProductResponse;
        }
    }
}
