using Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainServices.Interface
{
    public interface IProductService
    {
        IEnumerable<Products> SearchByCategoryName(string criteriaValue);
        IEnumerable<Products> GetProducts();
        string AddProduct(Products product);
    }
}
