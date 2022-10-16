using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class SqlServerProductRepository : IProductRepository
    {
        //EVAL: This doesn't feel like a particularly elegant solution. With more time, I may consider some sort
        //      of IProductValidator service.
        private const int NAME_MAX_LENGTH = 256;
        private const int DESCRIPTION_MAX_LENGTH = 256;

        public async Task<bool> AddProductAsync(Product product)
        {
            ValidateAddProductParameter(product, nameof(product));

            int productId = await AddOnlyProductAsync(product);

            product.InstanceId = productId;

            //Null coalesce into 0 to check for both null and empty collection in one condition
            if ((product.Metadata?.Count ?? 0) > 0)
                await AddProductMetadataAsync(product);

            return true;
        }

        private void ValidateAddProductParameter(Product product, string parameterName)
        {
            PreConditions.ParameterNotNull(product, parameterName);

            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));
            PreConditions.StringNotNullOrWhitespace(product.Description, nameof(product.Description));
            PreConditions.StringNotNullOrWhitespace(product.ProductImageUris, nameof(product.ProductImageUris));
            PreConditions.StringNotNullOrWhitespace(product.ValidSkus, nameof(product.ValidSkus));

            PreConditions.StringLengthDoesNotExceed(product.Name, NAME_MAX_LENGTH, nameof(product.Name));
            PreConditions.StringLengthDoesNotExceed(product.Description, DESCRIPTION_MAX_LENGTH, nameof(product.Description));
        }

        private Task<int> AddOnlyProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        private Task AddProductMetadataAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
