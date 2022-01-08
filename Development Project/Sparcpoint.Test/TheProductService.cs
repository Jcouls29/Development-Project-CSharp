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
    public class TheProductService
    {
        [TestMethod]
        public async Task GetsAllCurrentProducts()
        {
            var expectedReturnList = new List<Product>();

            Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
            mockDataService.Setup(p => p.GetProducts()).Returns(Task.FromResult(expectedReturnList));
            var productService = new ProductService(mockDataService.Object);
            var result = await productService.GetProducts();
            Assert.AreEqual(expectedReturnList, result);
        }

        [TestMethod]
        public async Task GetsAttributesForAProduct()
        {

            var expectedReturnList = new List<KeyValuePair<string, string>>();

            Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
            mockDataService.Setup(p => p.GetAttributesForProduct(It.IsAny<int>())).Returns(Task.FromResult(expectedReturnList));
            var productService = new ProductService(mockDataService.Object);
            var result = await productService.GetAttributesForProduct(5);
            Assert.AreEqual(expectedReturnList, result);
        }

        [TestClass]
        public class WhenCreatingAProduct
        {
            [TestMethod]
            public async Task SavesAProductToTheDataService()
            {
                Product calledWith = new Product();
                Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Callback<Product>(p => calledWith = p);
                var productService = new ProductService(mockDataService.Object);

                var productRequest = new CreateProductRequest()
                {
                    Name = "super awesome name",
                    Description = "The best description ever",
                    ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                    ValidSkus = new List<string>() { "1234", "5678" }
                };

                await productService.CreateProductAsync(productRequest);

                Assert.AreEqual(productRequest.Name, calledWith.Name);
                Assert.AreEqual(productRequest.Description, calledWith.Description);
                Assert.AreEqual(String.Join(",", productRequest.ProductImageUris.ToArray()), calledWith.ProductImageUris);
                Assert.AreEqual(String.Join(",", productRequest.ValidSkus.ToArray()), calledWith.ValidSkus);
            }

            [TestClass]
            public class GivenTheModelContainsAttributes
            {
                [TestMethod]
                public async Task CallsToAddTheAttributesToTheProduct()
                {
                    List<KeyValuePair<string, string>> calledWith = new List<KeyValuePair<string, string>>();
                    Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                    mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddAttributeToProduct(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>())).Callback<int, KeyValuePair<string, string>>((id, attribute) => calledWith.Add(attribute));
                    var productService = new ProductService(mockDataService.Object);

                    var productRequest = new CreateProductRequest()
                    {
                        Name = "super awesome name",
                        Description = "The best description ever",
                        ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                        ValidSkus = new List<string>() { "1234", "5678" },
                        ProductAttributes = new Dictionary<string, string>() {
                            { "Material", "Cotton" },
                            { "Brand", "Just Yarn" },
                            { "Length", "100 yd" }
                        }
                    };

                    await productService.CreateProductAsync(productRequest);

                    mockDataService.Verify(d => d.AddAttributeToProduct(5, It.IsAny<KeyValuePair<string, string>>()), Times.Exactly(3));
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(0).Key, calledWith.ElementAt(0).Key);
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(0).Value, calledWith.ElementAt(0).Value);
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(1).Key, calledWith.ElementAt(1).Key);
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(1).Value, calledWith.ElementAt(1).Value);
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(2).Key, calledWith.ElementAt(2).Key);
                    Assert.AreEqual(productRequest.ProductAttributes.ElementAt(2).Value, calledWith.ElementAt(2).Value);
                }
            }

            [TestClass]
            public class GivenTheModelDoesNotContainAttributes
            {
                [TestMethod]
                public async Task DoesNotCallToAddTheAttributesToTheProduct()
                {
                    Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                    mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddAttributeToProduct(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>())).Verifiable();
                    var productService = new ProductService(mockDataService.Object);

                    var productRequest = new CreateProductRequest()
                    {
                        Name = "super awesome name",
                        Description = "The best description ever",
                        ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                        ValidSkus = new List<string>() { "1234", "5678" }
                    };

                    await productService.CreateProductAsync(productRequest);

                    mockDataService.Verify(d => d.AddAttributeToProduct(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>()), Times.Never());
                }
            }
            [TestClass]
            public class GivenTheModelContainsCategories
            {
                [TestMethod]
                public async Task CallsToAddCateogiresToProduct()
                {
                    Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                    mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(4));
                    mockDataService.Setup(p => p.AddProductToCategory(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
                    var productService = new ProductService(mockDataService.Object);

                    var productRequest = new CreateProductRequest()
                    {
                        Name = "super awesome name",
                        Description = "The best description ever",
                        ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                        ValidSkus = new List<string>() { "1234", "5678" },
                        CategoryIds = new List<int>() {4, 7, 8 }
                    };

                    await productService.CreateProductAsync(productRequest);

                    mockDataService.Verify(d => d.AddProductToCategory(4, 4), Times.Once());
                    mockDataService.Verify(d => d.AddProductToCategory(7, 4), Times.Once());
                    mockDataService.Verify(d => d.AddProductToCategory(8, 4), Times.Once());

                }
            }

            [TestClass]
            public class GivenTheModelDoesNotContainCategories
            {
                [TestMethod]
                public async Task DoesNotCallToAddTheCategoriesToTheProduct()
                {
                    Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                    mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(4));
                    mockDataService.Setup(p => p.AddProductToCategory(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
                    var productService = new ProductService(mockDataService.Object);

                    var productRequest = new CreateProductRequest()
                    {
                        Name = "super awesome name",
                        Description = "The best description ever",
                        ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                        ValidSkus = new List<string>() { "1234", "5678" }
                    };

                    await productService.CreateProductAsync(productRequest);

                    mockDataService.Verify(d => d.AddProductToCategory(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
                }
            }
        }

        [TestClass]
        public class WhenAddingAttributesToProduct
        {
            [TestMethod]
            public async Task DoesNotCallToAddTheAttributesToTheProduct()
            {
                List<KeyValuePair<string, string>> calledWith = new List<KeyValuePair<string, string>>();

                Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(5));
                mockDataService.Setup(p => p.AddAttributeToProduct(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>())).Callback<int, KeyValuePair<string, string>>((id, attribute) => calledWith.Add(attribute));
                var productService = new ProductService(mockDataService.Object);

                var attributesToAdd = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Stuff", "Things"),
                    new KeyValuePair<string, string>("Neat", "Property"),
                    new KeyValuePair<string, string>("Shipping", "Standard")
                };
                await productService.AddAttributesToProduct(3, attributesToAdd);

                mockDataService.Verify(d => d.AddAttributeToProduct(3, It.IsAny<KeyValuePair<string, string>>()), Times.Exactly(3));
                Assert.AreEqual(attributesToAdd.ElementAt(0).Key, calledWith.ElementAt(0).Key);
                Assert.AreEqual(attributesToAdd.ElementAt(0).Value, calledWith.ElementAt(0).Value);
                Assert.AreEqual(attributesToAdd.ElementAt(1).Key, calledWith.ElementAt(1).Key);
                Assert.AreEqual(attributesToAdd.ElementAt(1).Value, calledWith.ElementAt(1).Value);
                Assert.AreEqual(attributesToAdd.ElementAt(2).Key, calledWith.ElementAt(2).Key);
                Assert.AreEqual(attributesToAdd.ElementAt(2).Value, calledWith.ElementAt(2).Value);
            }
        }

        [TestClass]
        public class WhenAddingProductToCategories
        {
            [TestMethod]
            public async Task CallsToAddCateogiresToProduct()
            {
                Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
                mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Returns(Task.FromResult(4));
                mockDataService.Setup(p => p.AddProductToCategory(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
                var productService = new ProductService(mockDataService.Object);


                await productService.AddProductToCategories(4, new List<int> { 7, 10, 13});

                mockDataService.Verify(d => d.AddProductToCategory(7, 4), Times.Once());
                mockDataService.Verify(d => d.AddProductToCategory(10, 4), Times.Once());
                mockDataService.Verify(d => d.AddProductToCategory(13, 4), Times.Once());

            }
        }

        
    }
}
