using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sparcpoint.Infrastructure.Persistence.Entities;
using Sparcpoint.Dto;
using AutoMapper;

namespace Sparcpoint.Implementations
{
    public class ProductService 
    {
        private IAsyncCollection<Product> _productCollection;
        private IAsyncCollection<InventoryTransaction> _transactionCollection;
        private IAsyncQueue<InventoryTransaction> _inventoryQueue;
        private readonly IMapper _mapper;

        public ProductService(IAsyncCollection<Product> productCollection, IAsyncQueue<InventoryTransaction> inventoryQueue, IAsyncCollection<InventoryTransaction> transactionCollection)
        {
            _productCollection = productCollection;
            _inventoryQueue = inventoryQueue;
            _transactionCollection = transactionCollection;
        }

        public async Task AddProduct(ProductDto product)
        {
            var productMapper = _mapper.Map<Product>(product);
            await _productCollection.Add(productMapper);
        }


        public IEnumerable<Product> GetAllProducts()
        {
            return _productCollection.ToList();
        }



        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            return  _productCollection.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToList();
        }

        public async Task AddToInventory(int productId, InventoryTransactionDto inventory)
        {
            var product = _productCollection.FirstOrDefault(p => p.InstanceId == productId);
            if (product != null)
            {
                var inventoryTran = _mapper.Map<InventoryTransaction>(inventory);
                _inventoryQueue.Add(inventoryTran);
            }
        }



        public async Task<int> Count(string input,CountType countType)
        {
            if (countType == CountType.PRODUCTID)
                return _productCollection.Count(p => p.InstanceId == Convert.ToInt32(input));
            var product = _productCollection.Count(p => p.ProductAttributes.Select(x => x.Value).Contains(input));
            return product;
        }


        public async Task<bool> RemoveTransaction(int id)
        {
            var check = _transactionCollection.FirstOrDefault(x=>x.TransactionId == id);
            if (check != null)
            {
                await _transactionCollection.Remove(check);
                return true;
            }
            return false;
        }


        public async Task<bool> AddProductToInventory(int id,InventoryTransactionDto inventory)
        {
            var check = _productCollection.FirstOrDefault(x => x.InstanceId == id);
            if (check != null)
            {
                var invMapper = _mapper.Map<InventoryTransaction>(inventory);
                invMapper.ProductInstanceId = id;
                _inventoryQueue.Add(invMapper);
                return true;
            }
            return false;
        }


        public void RemoveProduct(List<int> id)
        {
            var check = _transactionCollection.Where(x => id.Contains(x.ProductInstanceId)).ToList();
            if (check != null)
            {
                foreach (var item in check)
                {
                    _transactionCollection.Remove(item);
                }
                
            }
        }
    }

}
