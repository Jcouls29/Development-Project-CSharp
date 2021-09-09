using Interview.Data.DataAccess;
using Interview.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interview.Data.Contracts
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}


