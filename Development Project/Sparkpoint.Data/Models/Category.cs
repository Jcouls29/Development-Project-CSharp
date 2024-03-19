// <auto-generated>
// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparkpoint.Data
{
    // ****************************************************************************************************
    // This is not a commercial licence, therefore only a few tables/views/stored procedures are generated.
    // ****************************************************************************************************

    // Categories
    public class Category
    {
        public int InstanceId { get; set; } // InstanceId (Primary key)
        public string Name { get; set; } // Name (length: 64)
        public string Description { get; set; } // Description (length: 256)
        public DateTime CreatedTimestamp { get; set; } // CreatedTimestamp

        // Reverse navigation

        /// <summary>
        /// Child CategoryAttributes where [CategoryAttributes].[InstanceId] point to this entity (FK_CategoryAttributes_Categories)
        /// </summary>
        public ICollection<CategoryAttribute> CategoryAttributes { get; set; } // CategoryAttributes.FK_CategoryAttributes_Categories

        /// <summary>
        /// Child CategoryCategories where [CategoryCategories].[CategoryInstanceId] point to this entity (FK_CategoryCategories_Categories_Categories)
        /// </summary>
        public ICollection<CategoryCategory> CategoryCategories_CategoryInstanceId { get; set; } // CategoryCategories.FK_CategoryCategories_Categories_Categories

        /// <summary>
        /// Child CategoryCategories where [CategoryCategories].[InstanceId] point to this entity (FK_CategoryCategories_Categories)
        /// </summary>
        public ICollection<CategoryCategory> CategoryCategories_InstanceId { get; set; } // CategoryCategories.FK_CategoryCategories_Categories

        /// <summary>
        /// Child ProductCategories where [ProductCategories].[CategoryInstanceId] point to this entity (FK_ProductCategories_Categories)
        /// </summary>
        public ICollection<ProductCategory> ProductCategories { get; set; } // ProductCategories.FK_ProductCategories_Categories

        public Category()
        {
            CreatedTimestamp = DateTime.UtcNow;
            CategoryAttributes = new List<CategoryAttribute>();
            CategoryCategories_CategoryInstanceId = new List<CategoryCategory>();
            CategoryCategories_InstanceId = new List<CategoryCategory>();
            ProductCategories = new List<ProductCategory>();
        }
    }

}
// </auto-generated>