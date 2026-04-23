using Interview.Web.Models;
using System;

namespace Interview.Web.Services
{
    public class InventoryRule : IInventoryRule
    {
        public void ValidateAdd(AddInventoryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.ProductId <= 0)
                throw new ArgumentException("Invalid ProductId", nameof(request.ProductId));

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(request.Quantity));
        }

        public void ValidateRemove(RemoveInventoryRequest request, int currentInventory)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.ProductId <= 0)
                throw new ArgumentException("Invalid ProductId", nameof(request.ProductId));

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(request.Quantity));

            // EVAL: Prevent negative inventory (business rule)
            if (currentInventory - request.Quantity < 0)
                throw new InvalidOperationException("Insufficient inventory to remove requested quantity");
        }
    }
}
