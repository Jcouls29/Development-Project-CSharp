namespace Sparcpoint.Models.Requests
{
    public enum UpdateInventoryType
    {
        Add,
        Remove
    }

    public class UpdateInventoryRequest
    {
        public int ProductInstanceId { get; set; }
        public int UpdateBy { get; set; }
        public UpdateInventoryType Action { get; set; }
    }
}