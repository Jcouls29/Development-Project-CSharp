using Interview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interview.Data.Repository
{
    public class ProductRepository<T> : IRepository<T> where T : Product
    {
        private List<T> products;

        public ProductRepository()
        {
            products = new List<T>();
        }

        public virtual List<T> GetAll()
        {
            return products.ToList();
        }

        public virtual T GetByID(int id)
        {
            return products.FirstOrDefault(p => p.InstanceId == id);
        }

        public virtual void Add(T entity)
        {
            Insert(entity);
        }

        public virtual void Update(T entity)
        {
            var modelEntity = entity as Model.BaseModel;

            if (modelEntity != null)
            {
                modelEntity.UpdateDate = DateTime.UtcNow;
            }

            products.Remove(modelEntity as T);
            Insert(modelEntity as T);
        }

        public virtual void Delete(T entity)
        {
            products.Remove(entity);
        }

        private void Insert(T entity)
        {
            var modelEntity = entity as Model.BaseModel;

            if (modelEntity != null)
            {
                modelEntity.CreatedTimestamp = DateTime.UtcNow;
            }

            products.Add(modelEntity as T);
        }        
    }
}
