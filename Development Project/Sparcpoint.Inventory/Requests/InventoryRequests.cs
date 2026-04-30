namespace Sparcpoint.Inventory.Requests
{
    public class AddInventoryRequest
    {
        public int ProductInstanceId { get; set; }

        // EVAL: Always positive here — the repository layer negates for removals.
        // Keeping the API surface explicit avoids callers passing negative values accidentally.
        public decimal Quantity { get; set; }
        public string? TypeCategory { get; set; }
    }

    public class RemoveInventoryRequest
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string? TypeCategory { get; set; }
    }
}
