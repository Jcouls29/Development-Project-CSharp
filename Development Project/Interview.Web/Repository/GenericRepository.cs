using Interview.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.SqlServer.Abstractions;
using Interview.Data.Models;
using System.Data;
using AutoMapper;

namespace Interview.Web.Repository
{
    public class GenericRepository : IGenericRepository<ProductViewModel>
    {
        // EVAL: need db connection string
        public string constring = "";
        readonly ISqlExecutor sqlExecutor;
        private readonly IMapper _mapper;

        public GenericRepository(IMapper mapper)
        {
            _mapper = mapper;
            sqlExecutor = new SqlServerExecutor(constring);
        }

        private Product command(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            //EVAL: based on the connection string, get the connection
            return null;
        }

        Task<ProductViewModel> IGenericRepository<ProductViewModel>.Add(ProductViewModel entity)
        {
            Product product = _mapper.Map(entity, new Product());
            sqlExecutor.Execute(command);
            return Task.FromResult(entity);
        }

        Task<ProductViewModel> IGenericRepository<ProductViewModel>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        Task<ProductViewModel> IGenericRepository<ProductViewModel>.Get(int id)
        {
            throw new NotImplementedException();
        }

        Task<List<ProductViewModel>> IGenericRepository<ProductViewModel>.GetAll()
        {
            // EVAL: view model of products
            var vmProducts = new List<ProductViewModel>();
            //EVAL: make a db call and get the products 
            var products = new List<Product>();
            var productsDTO = _mapper.Map(products, vmProducts);
            return Task.FromResult(productsDTO);
        }

        Task<ProductViewModel> IGenericRepository<ProductViewModel>.Update(ProductViewModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
