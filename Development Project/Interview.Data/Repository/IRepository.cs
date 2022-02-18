using System;
using System.Collections.Generic;
using System.Text;

namespace Interview.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetByID(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
