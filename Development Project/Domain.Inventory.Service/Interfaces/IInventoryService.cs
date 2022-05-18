// <copyright file="IInventoryService.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Inventory.Service.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Models;

    /// <summary>
    /// The interace for the inventory service.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Searches for products asynchronous.
        /// </summary>
        /// <param name="toSearchFor">To search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Exception on failure.</returns>
        Task<IEnumerable<Product>> SearchForProductsAsync(MetaData toSearchFor, CancellationToken cancellationToken = default(CancellationToken));
    }
}
