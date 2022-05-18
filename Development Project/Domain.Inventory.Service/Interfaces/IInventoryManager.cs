// <copyright file="IInventoryManager.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Inventory.Service.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Models;

    /// <summary>
    /// Used to interface deal with inventory.
    /// </summary>
    public interface IInventoryManager
    {
        /// <summary>
        /// Finds the product on metadata asynchronous.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <returns>Exception on failure.</returns>
        Task<IEnumerable<Product>> FindProductOnMetadataAsync(MetaData toFind, System.Threading.CancellationToken cancellationToken);
    }
}
