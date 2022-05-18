// <copyright file="DIConfig.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Configuration
{
    using System;
    using System.Data;

    using Domain.Inventory.Impl.DB;
    using Domain.Inventory.Service.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Used to configure DI.
    /// </summary>
    public static class DIConfig
    {
        /// <summary>
        /// Configurations the di.
        /// </summary>
        /// <param name="sc">The sc.</param>
        /// <param name="config">The configuration.</param>
        public static void ConfigDI(this IServiceCollection sc, IConfiguration config)
        {
            sc.AddTransient<IDbConnection>((IServiceProvider sp) =>
            {
                // Ran out of time.
                throw new NotImplementedException();
            });
            sc.AddTransient<IInventoryManager, DBInventoryManager>();
            sc.AddTransient<IInventoryService, IInventoryService>();
        }
    }
}
