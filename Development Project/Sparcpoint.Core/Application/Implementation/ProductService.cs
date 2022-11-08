using FluentValidation.Results;
using Sparcpoint.Abstract.Products;
using Sparcpoint.Application.Abstract;
using Sparcpoint.Models.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sparcpoint.Models.Products.ValidationRules;

namespace Sparcpoint.Application.Implementation
{
    public class ProductService: IProductService
    {
        //EVAL we can add different client product repositories
        private readonly IBaseProductRepository _baseProductRepository;
        public ProductService(IBaseProductRepository baseProductRepository)
        {
            _baseProductRepository = baseProductRepository;
        }

        #region Base Product 
        public async Task<int> CreateBaseProductServiceAsync(BaseProduct addProduct, string consumerId)
        {
            try
            {
                //EVAL : This method can be used to run different business rules for different consumers
                // without affecting one another
                ValidationResult validationResults = null;
                switch (consumerId)
                {
                    case "Client1":
                        validationResults = await new Consumer1ProductValidator().ValidateAsync(addProduct, default(CancellationToken));
                        break;

                    default:
                        validationResults = await new DefaultProductValidator().ValidateAsync(addProduct, default(CancellationToken));
                        break;
                }

                if (validationResults.IsValid)
                {
                    return await _baseProductRepository.CreateAsync(addProduct);
                }
                else
                {
                    //EVAL : we can also send the exact validation result mentioned in the rules 
                    //back to the user
                    throw new ApplicationException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BaseProduct>> GetBaseProductsServiceAsync(BaseProduct searchProduct, string consumerId)
        {
            try
            {
                //Based on client id run business rules if they apply

                return await _baseProductRepository.GetAllAsync(searchProduct);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
