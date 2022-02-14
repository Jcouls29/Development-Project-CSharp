using Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dal.Interfaces
{
    public interface IProductRepository
    {
        List<Products> GetProducts();
        List<Products> SearchProductsByCategory(string searchCriteria);
        void UpdateProductCategories(List<int> categoriesId, int productId);
        public List<int> AddCategories(Products product);
        public int AddProduct(Products product);
    }
}
