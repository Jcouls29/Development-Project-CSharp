// EVAL: This file re-exports the shared InMemoryInventoryRepository from Sparcpoint.Inventory
// so existing test code continues to compile without changes to using statements.
using Sparcpoint.Inventory.Repositories.InMemory;

namespace Sparcpoint.Inventory.Tests.Fakes
{
    /// <summary>
    /// Test-specific alias for the shared in-memory inventory repository.
    /// </summary>
    public class InMemoryInventoryRepository : Sparcpoint.Inventory.Repositories.InMemory.InMemoryInventoryRepository
    {
        public InMemoryInventoryRepository(Sparcpoint.Inventory.Repositories.IProductRepository productRepository)
            : base(productRepository)
        {
        }
    }
}
