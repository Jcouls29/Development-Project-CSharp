// <copyright file="InventoryService.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Inventory.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Inventory.Service.Interfaces;
    using Domain.Models;

    /// <summary>
    /// Used for working with inventory.
    /// </summary>
    public class InventoryService
        : IInventoryService
    {
        private IInventoryManager InvMgr;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="inventoryManager">The inventory manager.</param>
        public InventoryService(IInventoryManager inventoryManager)
        {
            this.InvMgr = inventoryManager;

            this.ValidateInst();
        }

        /// <summary>
        /// Searches for products asynchronous.
        /// </summary>
        /// <param name="toSearchFor">The metadata to search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Exception on failure.</returns>
        /// <exception cref="System.ArgumentNullException">toSearchFor</exception>
        public async Task<IEnumerable<Product>> SearchForProductsAsync(MetaData toSearchFor, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<Product> ret = null;

            //TODO: Use common validation.
            if (toSearchFor == null)
            {
                throw new ArgumentNullException(nameof(toSearchFor));
            }

            try
            {
                ret = await this.InvMgr.FindProductOnMetadataAsync(toSearchFor, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!this.HandleError(ex))
                {
                    throw;
                }
            }

            return ret;
        }

        private bool HandleError(Exception ex)
        {
            // TODO: use common service code.
            return false;
        }

        private void ValidateInst()
        {
            // TODO: Used common validation.

            if (this.InvMgr == null)
            {
                throw new ArgumentNullException("The inventory manager was null.");
            }
        }
    }
}
