using Interview.Web.Core.Repositories;
using Interview.Web.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace Interview.Web.DataServices.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly SparcpointContext Context;
        public Repository(SparcpointContext context)
        {
            Context = context;
        }
        public TEntity Add(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
                Context.SaveChanges();
                return entity;
            }catch(Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().Where(x => true);
        }
    }
}
