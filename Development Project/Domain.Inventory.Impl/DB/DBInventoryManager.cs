// <copyright file="DBInventoryManager.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Inventory.Impl.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Inventory.Service.Interfaces;
    using Domain.Models;

    /// <summary>
    /// The database implementation of the inventory manager.
    /// </summary>
    /// <seealso cref="Domain.Inventory.Service.Interfaces.IInventoryManager" />
    public class DBInventoryManager
        : IInventoryManager
    {
        private IDbConnection dbConn;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBInventoryManager"/> class.
        /// </summary>
        /// <param name="dbConnection">The database connection.</param>
        public DBInventoryManager(IDbConnection dbConnection)
        {
            this.dbConn = dbConnection;

            this.ValidateInst();
        }

        /// <summary>
        /// Finds the product on metadata using the database.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// Exception on failure.
        /// </returns>
        public Task<IEnumerable<Product>> FindProductOnMetadataAsync(MetaData toFind, CancellationToken cancellationToken)
        {
            List<Product> ret = new List<Product>();

            // TODO: Plug in the DB.

            return Task.FromResult<IEnumerable<Product>>(ret);
        }

        private void ValidateInst()
        {
            // TODO: Use a common validation lib.
            if (this.dbConn == null)
            {
                throw new ArgumentNullException(nameof(this.dbConn));
            }
        }
    }
}
