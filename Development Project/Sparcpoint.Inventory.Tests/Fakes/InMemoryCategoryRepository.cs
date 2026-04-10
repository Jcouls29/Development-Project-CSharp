// EVAL: This file re-exports the shared InMemoryCategoryRepository from Sparcpoint.Inventory
// so existing test code continues to compile without changes to using statements.
using Sparcpoint.Inventory.Repositories.InMemory;

namespace Sparcpoint.Inventory.Tests.Fakes
{
    /// <summary>
    /// Test-specific alias for the shared in-memory category repository.
    /// </summary>
    public class InMemoryCategoryRepository : Sparcpoint.Inventory.Repositories.InMemory.InMemoryCategoryRepository
    {
    }
}
