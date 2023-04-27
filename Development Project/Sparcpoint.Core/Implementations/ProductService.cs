using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;

namespace Sparcpoint.Implementations
{
    public class ProductService : IProductService
    {
        public ProductService() { }

        public HttpResponseMessage GetProducts()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public ActionResult AddProduct(string jsonBody)
        {
            JsonDataSerializer serializer = new JsonDataSerializer();
            Product product = (Product) serializer.Deserialize(typeof(Product), jsonBody);
            return null;
        }

        public ActionResult SearchProduct(string product)
        {
            JsonDataSerializer serializer = new JsonDataSerializer();
            return null;
        }

        public ActionResult AddInventory()
        {
            return null;
        }

        public ActionResult RemoveInventory()
        {
            return null;
        }

        public ActionResult GetProductCount()
        {
            return null;
        }
    }
}
