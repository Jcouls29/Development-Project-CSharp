namespace Sparcpoint.Inventory.Domain.Entities
{
    public class Entity<TKey>
    {
        protected virtual TKey Id { get; set; }
    }

    public class Entity : Entity<int> { }
}
