using DomainServices.Interface;
using Microsoft.Extensions.Configuration;
using Dal.Models;
using Sparcpoint;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DomainServices
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IDataSerializer _serializer;
        public ProductService(IConfiguration configuration, IDataSerializer serializer)
        {
            _configuration = configuration;
            _serializer = serializer;
        }

        public IEnumerable<Products> SearchByCategoryName(string criteriaValue)
        {
            var url = _configuration.GetValue<string>("ApiUrl");
            WebClient client = new WebClient();
            client.Headers["content-type"] = "application/json";
            client.Encoding = System.Text.Encoding.UTF8;
            var response = client.DownloadString(url + $"/product/searchByCategory?searchCriteria={criteriaValue}");
            var products = new JsonDataSerializer().Deserialize<List<Products>>(response);
            return products;
        }

        public IEnumerable<Products> GetProducts()
        {
            var url = _configuration.GetValue<string>("ApiUrl");
            WebClient client = new WebClient();
            client.Headers["content-type"] = "application/json";
            client.Encoding = System.Text.Encoding.UTF8;
            var response = client.DownloadString(url + "/product");
            var products = new JsonDataSerializer().Deserialize<List<Products>>(response);
            return products;
        }
        public string AddProduct(Products product)
        {
            var url = _configuration.GetValue<string>("ApiUrl");

            var inputJson = new JsonDataSerializer().Serialize(product);

            WebClient client = new WebClient();
            client.Headers["content-type"] = "application/json";
            client.Encoding = System.Text.Encoding.UTF8;
            string json = client.UploadString(url + "/product", inputJson);
            return json;
        }
    }
}
