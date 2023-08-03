using System.Collections.Generic;

namespace Interview.Web.Services.interfaces
{
    public interface IProductService
    {
        void AddProduct(string name, string description, decimal price, List<string> categoryNames, Dictionary<string, string> metadata);
    }
}
