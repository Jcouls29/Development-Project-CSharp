using System;
using System.Collections.Generic;
using System.Linq;
using Interview.Data.Contracts;
using Interview.Data.Models;

namespace Interview.Data.DataAccess
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
        
        public virtual T GetById(int id)
        {
            return products.FirstOrDefault(p => p.ID == id);                     
        }    

        public virtual void Add(T entity)
        {
            Insert(entity);
        }

        public virtual void Update(T entity)
        {
            var modelEntity = entity as Models.ModelBase;

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
            var modelEntity = entity as Models.ModelBase;

            if (modelEntity != null)
            {
                modelEntity.CreateDate = DateTime.UtcNow;
            }

            products.Add(modelEntity as T);
        }
    }
}