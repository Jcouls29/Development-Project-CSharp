using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    public class ProductRepository<T> : IRepository<T> where T : Product
    {
        private List<T> products;
        public ProductRepository()
        {
            products = new List<T>();
        }
        public void Delete(Guid id)
        {
            var product = FindById(id);
            if (product is null)
            {
                // implement custome exception
                throw new Exception("Product Not found");
            }
            product.Active = false;
        }

        public IEnumerable<T> GetAll()
        {
            return products.AsEnumerable<T>();
        }

        public T GetById(Guid id)
        {
            return FindById(id);
        }

        public void Insert(T entity)
        {
            if(entity is null)
            {
                throw new Exception("Invalid entity");
            }
            products.Add(entity);
        }

        public void Update(T entity)
        {
            if (entity is null)
            {
                throw new Exception("Invalid entity");
            }

            var product = FindById(entity.id);
            products.Remove(product);
            Insert(entity);
        }

        private T FindById(Guid id)
        {
            return products.Find(p => p.id == id);
        }
    }
}
