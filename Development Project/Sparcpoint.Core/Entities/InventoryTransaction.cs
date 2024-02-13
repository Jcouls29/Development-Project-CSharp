namespace Sparcpoint.Core.Entities;

public record InventoryTransaction
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public decimal Quantity { get; init; }

    public DateTime StartDate { get; init; }
    public DateTime? CompletedDate { get; init; }

    public Product Product { get; init; } = null!;
}
