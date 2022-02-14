using DomainServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dal.Models;
using Sparcpoint;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Interview.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            //Get Products:
            var products = _productService.GetProducts();

            products = products == null ? Enumerable.Empty<Products>() : products;
            //If product is null - return empty products
            return View(products);
        }


        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        // GET: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Products product)
        {
            int productId = int.Parse(_productService.AddProduct(product));
            if (productId >= 0)
            {
                return View("Index", _productService.GetProducts());
            } else
            {
                return View("Index", Enumerable.Empty<Products>());
            }
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search()
        {
            var search = new Search
            {
                Products = _productService.GetProducts().ToList(),
                SearchCriteria = new List<SearchCriteria>
                {
                    new SearchCriteria
                    {
                        Id = 1,
                        Name = "Category"
                    },
                    new SearchCriteria
                    {
                        Id = 2,
                        Name = "Metadata"
                    }
                }
            };

            return View("Search", search);
        }

        //POST: ProductController/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(IFormCollection searchParameters)
        {
            string selectedCriteria = searchParameters["SelectedCriteria"].ToString();

            string criteriaValue = searchParameters["CriteriaValue"].ToString();

            var products = _productService.SearchByCategoryName(criteriaValue);
            try
            {
                return View("Index", products);
            }
            catch
            {
                return View();
            }
        }

    }
}
