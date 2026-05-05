namespace Interview.Web.Models.Requests
{
    public class BulkModifyInventoryRequest
    {
        public ModifyInventoryRequest[] Items { get; set; }
    }
}
