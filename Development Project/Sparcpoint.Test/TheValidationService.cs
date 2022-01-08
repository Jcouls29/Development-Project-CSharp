using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.DataServices;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Test
{
    [TestClass]
    public class TheValidationService
    {
        [TestClass]
        public class WhenValidatingCateogries
        {
            [TestMethod]
            public void RequiresAName()
            {
                var validationService = new ValidationService();

                var invalidCat = new CreateCategoryRequest();

                var response = validationService.CategoryIsValid(invalidCat);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Name is required", response.InvalidMessage);
            }

            [TestMethod]
            public void RequiresADescription()
            {
                var validationService = new ValidationService();

                var invalidCat = new CreateCategoryRequest() { Name = "Test" };

                var response = validationService.CategoryIsValid(invalidCat);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Description is required", response.InvalidMessage);
            }

            [TestMethod]
            public void IndicatesIfCategoryIsValid()
            {
                var validationService = new ValidationService();

                var invalidCat = new CreateCategoryRequest() { Name = "Test", Description = "desc" };

                var response = validationService.CategoryIsValid(invalidCat);
                Assert.IsTrue(response.IsValid);
                Assert.IsNull(response.InvalidMessage);
            }
        }

        [TestClass]
        public class WhenValidatingQuantity
        {
            [TestMethod]
            public void RequiresQuantityIsPositive()
            {
                var validationService = new ValidationService();

                var response = validationService.QuantityIsValid(-2);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Quantity of inventory must be positive", response.InvalidMessage);
            }

            [TestMethod]
            public void IndicatesIfQuantityIsValid()
            {
                var validationService = new ValidationService();

                var response = validationService.QuantityIsValid(1);
                Assert.IsTrue(response.IsValid);
                Assert.IsNull(response.InvalidMessage);
            }
        }

        [TestClass]
        public class WhenValidatingProduct
        {
            [TestMethod]
            public void RequiresAName()
            {
                var validationService = new ValidationService();

                var invalidProduct = new CreateProductRequest();

                var response = validationService.ProductIsValid(invalidProduct);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Name is required", response.InvalidMessage);
            }

            [TestMethod]
            public void RequiresADescription()
            {
                var validationService = new ValidationService();

                var invalidProduct = new CreateProductRequest() { 
                    Name = "Product"
                };

                var response = validationService.ProductIsValid(invalidProduct);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Description is required", response.InvalidMessage);
            }

            [TestMethod]
            public void RequiresAUri()
            {
                var validationService = new ValidationService();

                var invalidProduct = new CreateProductRequest()
                {
                    Name = "Product",
                    Description = "desc",
                };

                var response = validationService.ProductIsValid(invalidProduct);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("At least one image uri is required", response.InvalidMessage);
            }

            [TestMethod]
            public void RequiresAValidSKU()
            {
                var validationService = new ValidationService();

                var invalidProduct = new CreateProductRequest()
                {
                    Name = "Product",
                    Description = "desc",
                    ProductImageUris = new List<string>() { "www.google.com" }
                };

                var response = validationService.ProductIsValid(invalidProduct);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("At least one sku is required", response.InvalidMessage);
            }

            [TestMethod]
            public void IndicatesIfProductIsValid()
            {
                var validationService = new ValidationService();

                var validProduct = new CreateProductRequest()
                {
                    Name = "Product",
                    Description = "desc",
                    ProductImageUris = new List<string>() { "www.google.com" },
                    ValidSkus = new List<string>() { "1234" }
                };

                var response = validationService.ProductIsValid(validProduct);
                Assert.IsTrue(response.IsValid);
                Assert.IsNull(response.InvalidMessage);
            }
        }

        [TestClass]
        public class WhenValidatingSearch
        {
            [TestMethod]
            public void RequiresAKeyword()
            {
                var validationService = new ValidationService();

                var invalidSearch = new ProductSearchRequest() { };

                var response = validationService.SearchIsValid(invalidSearch);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("Keyword is required", response.InvalidMessage);
            }

            [TestMethod]
            public void RequiresAValidSearchBy()
            {
                var validationService = new ValidationService();

                var invalidSearch = new ProductSearchRequest() { 
                    Keyword = "test",
                    SearchBy = new List<string>() { "test" }
                };

                var response = validationService.SearchIsValid(invalidSearch);
                Assert.IsFalse(response.IsValid);
                Assert.AreEqual("The search by field is not a valid option", response.InvalidMessage);
            }

            [TestMethod]
            public void IndicatesIfSearchIsValid()
            {
                var validationService = new ValidationService();

                var validSearch = new ProductSearchRequest()
                {
                    Keyword = "test"
                };

                var response = validationService.SearchIsValid(validSearch);
                Assert.IsTrue(response.IsValid);
                Assert.IsNull(response.InvalidMessage);


                validSearch = new ProductSearchRequest()
                {
                    Keyword = "test",
                    SearchBy = new List<string>() { "All" }
                };

                response = validationService.SearchIsValid(validSearch);
                Assert.IsTrue(response.IsValid);
                Assert.IsNull(response.InvalidMessage);
            }
        }
    }
}
