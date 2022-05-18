using Domain.Inventory.Service.Interfaces;
using Domain.Models;
using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Impl
{
    public class InventoryControllerImpl
        : IController
    {
        private IInventoryService service;

        public InventoryControllerImpl(IInventoryService service)
        {
            this.service = service;

            this.ValidateInst();
        }

        public Task<ActionResult<Product>> AddProductAsync()
        {
            // TODO: Implement in the service.
            return Task.FromResult<ActionResult<Product>>(new OkObjectResult(new Product()));
        }

        public async Task<ActionResult<Product>> FindProductOnMetadataAsync(MetaData body)
        {
            return new OkObjectResult(await this.service.SearchForProductsAsync(body));
        }

        private void ValidateInst()
        {
            if (this.service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
        }
    }
}
