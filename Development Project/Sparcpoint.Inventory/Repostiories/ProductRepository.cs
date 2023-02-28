using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repostiories
{
    public class ProductRepository : RepositoryBase<Product>
    {
        public ProductRepository(DbContext context) : base(context)
        {
        }
    }

    public class CategoryRepository : RepositoryBase<Category>
    {
        public CategoryRepository(DbContext context) : base(context)
        {
        }
    }
}
