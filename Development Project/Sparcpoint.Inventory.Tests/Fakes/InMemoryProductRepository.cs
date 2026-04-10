// EVAL: This file re-exports the shared InMemoryProductRepository from Sparcpoint.Inventory
// so existing test code continues to compile without changes to using statements.
// The real implementation lives in Sparcpoint.Inventory.Repositories.InMemory.
using Sparcpoint.Inventory.Repositories.InMemory;

namespace Sparcpoint.Inventory.Tests.Fakes
{
    /// <summary>
    /// Test-specific alias for the shared in-memory product repository.
    /// </summary>
    public class InMemoryProductRepository : Sparcpoint.Inventory.Repositories.InMemory.InMemoryProductRepository
    {
    }
}
