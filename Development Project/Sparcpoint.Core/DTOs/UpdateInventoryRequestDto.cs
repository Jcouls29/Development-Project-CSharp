namespace Sparcpoint.DTOs
{
    public class UpdateInventoryRequestDto
    {
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public string TypeCategory { get; set; }

    }
}
