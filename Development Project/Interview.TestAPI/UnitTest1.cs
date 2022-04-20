using Interview.Web.Controllers;
using Interview.Web.Models;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using System.Text.Json;

namespace Interview.TestAPI
{
  public class UnitTest1
  {
    [Fact]
    public async void GetAllProducts()
    {
      //Hosted web API REST Service base url
      string Baseurl = "https://localhost:5001/";

      using (var client = new HttpClient())
      {
        //Passing service base url
        client.BaseAddress = new Uri(Baseurl);

        client.DefaultRequestHeaders.Clear();
        //Define request data format
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //Sending request to find web api REST service resource GetAllProducts using HttpClient
        HttpResponseMessage Res = await client.GetAsync("api/v1/products");

        //Checking the response is successful or not which is sent using HttpClient
        if (Res.IsSuccessStatusCode)
        {
          //Storing the response details recieved from web api 
          var resplose = Res.Content.ReadAsStringAsync().Result;

          //Deserializing the response recieved from web api and storing into the Product list
          var res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Product>>(resplose);
          Assert.NotNull(res);


        }
        else
        {
          Assert.True(false, "Not working");
        }
      }

    }
    [Fact]
    public async void PutProducts()
    {
      //Hosted web API REST Service base url
      string Baseurl = "https://localhost:5001/";

      var product = new Product()
      {
        InstanceId = 0,
        Name = "NewProd",
        Description = "NewDesc",
        ProductImageUris = "ada,asdfas,asdf",
        ValidSkus = "asdfs,sdafd,dsafsad"
      };
      using (var client = new HttpClient())
      {
        //Passing service base url
        client.BaseAddress = new Uri(Baseurl);

        client.DefaultRequestHeaders.Clear();
        //Define request data format

        string inputJson = Newtonsoft.Json.JsonConvert.SerializeObject(product);
        HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
        HttpResponseMessage Res = client.PutAsync("api/v1/products", inputContent).Result;

        //Checking the response is successful or not which is sent using HttpClient
        if (Res.IsSuccessStatusCode)
        {
          //Storing the response details recieved from web api 
          var prodId = Res.Content.ReadAsStringAsync().Result;

          var savedProd = GetProduct(prodId);
          Assert.True(savedProd != null);
          Assert.True(savedProd.Name == product.Name);
          Assert.True(savedProd.Description == product.Description);
          Assert.True(savedProd.ValidSkus == product.ValidSkus);
          Assert.True(savedProd.ProductImageUris == product.ProductImageUris);

        }
        else
        {
          Assert.True(false, "Not working");
        }
      }

    }
    public Product GetProduct(string Id)
    {
      //Hosted web API REST Service base url
      string Baseurl = "https://localhost:5001/";

      using (var client = new HttpClient())
      {
        //Passing service base url
        client.BaseAddress = new Uri(Baseurl);

        client.DefaultRequestHeaders.Clear();
        //Define request data format
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //Sending request to find web api REST service resource GetProduct using HttpClient
        HttpResponseMessage Res = client.GetAsync($"api/v1/products/{Id}").Result;

        //Checking the response is successful or not which is sent using HttpClient
        if (Res.IsSuccessStatusCode)
        {
          //Storing the response details recieved from web api 
          var resplose = Res.Content.ReadAsStringAsync().Result;

          //Deserializing the response recieved from web api and storing into the Product
          var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(resplose);
          return res;
        }
        //returning the product 
      }
      return null;

    }
  }
}
