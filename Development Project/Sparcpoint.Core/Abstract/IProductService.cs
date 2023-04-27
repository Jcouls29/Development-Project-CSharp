using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;

namespace Sparcpoint
{
    public interface IProductService
    {
        HttpResponseMessage GetProducts();

        ActionResult AddProduct(string jsonBody);

        ActionResult SearchProduct(string product);

        ActionResult AddInventory();

        ActionResult RemoveInventory();

        ActionResult GetProductCount();
    }
}
