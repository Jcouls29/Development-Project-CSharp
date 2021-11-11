using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Interview.Web.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
    }
}