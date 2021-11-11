using Interview.Web.Core.DomainModel;
using Interview.Web.Core.Repositories;
using Interview.Web.Core.Services;
using Interview.Web.Entities;
using System.Collections.Generic;

namespace Interview.Web.DataServices.Services.Product
{
    public class Product : IProduct
    {
        internal readonly IUnitOfWork _unitOfWork;
        public Product(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ProductResponse AddProduct(ProdusctRequest products)
        {
            Entities.Products product = new Entities.Products();
            product.Name = products.Name;
            product.Description = products.Description;
            product.ValidSkus = products.ValidSkus;
            product.ProductImageUris = products.ProductImageUris;
            product.CreatedTimestamp = System.DateTime.Now;
            product.StartDate = products.StartDate;
            product.EndDate = products.EndDate;
            ICollection<ProductAttributes> ProductAttributes = new List<ProductAttributes>();
            foreach (ProductAttributeRequest item in products.ProductAttributes)
            {
                ProductAttributes.Add(new ProductAttributes() { Key = item.Key, Value = item.Value });
            }
            product.ProductAttributes = ProductAttributes;


            var entity = _unitOfWork.ProductRepository.Add(product);

            ProductResponse productResponse = new ProductResponse();
            productResponse.Products = entity;

            List<ResponseMessage> rm = new List<ResponseMessage>();
            if (entity != null)
            {
                rm.Add(new ResponseMessage() { ReturnCode = 200, ReturnText = "Product Created" });
            }
            else
            {
                rm.Add(new ResponseMessage() { ReturnCode = 200, ReturnText = "Product Created" });
            }
            productResponse.ResponseMessage = rm;
            return productResponse;
        }

        public IEnumerable<Entities.Products> SearchProduct()
        {
            return _unitOfWork.ProductRepository.GetAll();
        }
    }
}
