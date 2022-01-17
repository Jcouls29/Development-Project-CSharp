using SparcPoint.Inventory.DataModels;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.BL
{
    public class ProductsCollection : IAsyncCollection<Product>
    {
        private readonly ConcurrentBag<Product> _products;
        public ProductsCollection()
        {
            _products = new ConcurrentBag<Product>();
        }

        public Task Add(Product item)
        {
            return Task.Run(() =>
            {
                if (item == null)
                    _products.Add(item);
            });
        }


        public Task Clear()
        {
            return Task.Run(() =>
            {
                while(_products.Count > 0)
                {
                    var _p = new Product();
                    _products.TryTake(out _p);                    
                }
                
            });
        }

        public Task<int> GetCount()
        {
            return Task.Run((() =>
            {
                return _products.Count;
            }));
        }

        public IEnumerator<Product> GetEnumerator()
        {
            return (IEnumerator<Product>)Task.Run(() =>
            {
                return _products.ToArray();
            });
        }

        public Task<bool> Remove(Product item)
        {
            var result = _products.TryTake(out item);
            return Task.FromResult(result);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _products.ToArray();
        }
    }
}
