using Sparcpoint.Core.Entities;

namespace Sparcpoint.Api.Responses;

public class ProductTransactionResponse
{
    public List<InventoryTransaction> Transactions { get; set; } = [];
    public decimal TotalQuantity { get; set; }
}
