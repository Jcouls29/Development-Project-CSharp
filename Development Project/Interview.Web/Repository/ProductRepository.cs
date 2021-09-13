using Interview.Web.models;

namespace Interview.Web.Repository
{
    public class ProductRepository : IProductRepository
    {
        private List<T> products;

        /// <summary>
        /// 
        /// </summary>
        public ProductRepository()
        {
            products = new List<T>();
        }

        /// <summary>
        /// Retrieves all products
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetAll()
        {
            return products.ToList();
        }

        /// <summary>
        /// get by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return products.FirstOrDefault(p => p.ID == id);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(T entity)
        {
            Insert(entity);
        }

        /// <summary>
        /// Updates
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            var modelEntity = entity as Models.BaseModel;

            if (modelEntity != null)
            {
                modelEntity.UpdateDate = DateTime.UtcNow;
            }

            products.Remove(modelEntity as T);
            Insert(modelEntity as T);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            products.Remove(entity);
        }

        /// <summary>
        /// Inserts
        /// </summary>
        /// <param name="entity"></param>
        private void Insert(T entity)
        {
            var modelEntity = entity as Models.BaseModel;

            if (modelEntity != null)
            {
                modelEntity.CreateDate = DateTime.UtcNow;
            }

            products.Add(modelEntity as T);
        }
    }
}

