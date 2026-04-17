namespace Sparcpoint.DTOs
{
    public class UpdateInventoryRequestDto
    {
        public int ProductInstanceId { get; set; }

        public decimal Quantity { get; set; }

        public string TypeCategory { get; set; }

    }
}
