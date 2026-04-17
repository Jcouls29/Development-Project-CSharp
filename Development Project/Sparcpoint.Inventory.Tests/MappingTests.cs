using System.Collections.Generic;
using Sparcpoint.Inventory.Models;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    /// <summary>
    /// EVAL: Smoke tests on POCOs to guarantee that lists are not
    /// null by default (avoids NullReferenceException in lazy code-paths).
    /// </summary>
    public class ModelDefaultsTests
    {
        [Fact]
        public void Product_Has_Empty_Lists_By_Default()
        {
            var p = new Product();
            Assert.NotNull(p.Attributes);
            Assert.NotNull(p.CategoryIds);
            Assert.NotNull(p.ProductImageUris);
            Assert.NotNull(p.ValidSkus);
        }

        [Fact]
        public void Category_Has_Empty_Lists_By_Default()
        {
            var c = new Category();
            Assert.NotNull(c.Attributes);
            Assert.NotNull(c.ParentCategoryIds);
        }

        [Fact]
        public void SearchCriteria_Defaults_Are_Sensible()
        {
            var s = new ProductSearchCriteria();
            Assert.Equal(0, s.Skip);
            Assert.Equal(100, s.Take);
            Assert.Empty(s.AttributeFilters);
            Assert.Empty(s.CategoryIds);
        }

        [Fact]
        public void InventoryCountQuery_DefaultsExcludeReverted()
        {
            var q = new InventoryCountQuery();
            Assert.False(q.IncludeReverted);
            Assert.Null(q.ProductInstanceId);
        }
    }
}
