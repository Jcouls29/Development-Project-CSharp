namespace Sparcpoint.Inventory.Extensions
{
    public sealed class InventoryOptions
    {
        public const string SectionName = "Inventory";

        public string? ConnectionString { get; set; }

        /// EVAL: Flip to use in-memory repos — removes the SQL dependency
        /// entirely (useful for local dev, demos, and integration tests).
        public bool UseInMemoryRepositories { get; set; } = false;
    }
}
