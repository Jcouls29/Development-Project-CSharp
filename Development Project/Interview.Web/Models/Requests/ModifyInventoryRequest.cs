namespace Interview.Web.Models.Requests
{
    public class ModifyInventoryRequest
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}
