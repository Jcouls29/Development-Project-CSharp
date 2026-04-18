namespace Sparcpoint.Inventory.Application.Dtos.Requests
{
    public class InventoryRequest
    {
        public List<int> ProductIds { get; set; }
        public decimal Quantity { get; set; }
    }
}