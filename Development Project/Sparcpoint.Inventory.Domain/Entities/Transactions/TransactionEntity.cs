namespace Sparcpoint.Inventory.Domain.Entities.Transactions
{
    public abstract class TransactionEntity : Entity
    {
        protected override int Id { get; set; } = int.MinValue;
        public int TransactionId { get => Id; set => Id = value; }
    }
}
