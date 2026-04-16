namespace Sparcpoint.Inventory.Application.Dtos.Requests
{
    public class SearchProductsRequest
    {
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
