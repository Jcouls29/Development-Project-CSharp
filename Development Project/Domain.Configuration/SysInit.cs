// <copyright file="SysInit.cs" company="Intellectual Technologies">
// Copyright (c) Intellectual Technologies. All rights reserved.
// </copyright>

namespace Domain.Configuration
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The system init code.
    /// </summary>
    public static class Sys
    {
        /// <summary>
        /// Initializes the system.
        /// </summary>
        /// <param name="toInit">To initialize.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>ServiceCollection with DI initialized.</returns>
        public static IServiceCollection Init(this IServiceCollection toInit, IConfiguration config)
        {
            // TODO: Configure logging.
            toInit.ConfigDI(config);

            return toInit;
        }
    }
}
