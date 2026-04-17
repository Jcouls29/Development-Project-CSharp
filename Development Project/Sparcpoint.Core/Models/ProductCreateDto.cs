namespace Sparcpoint.Core.Models
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; }
        public System.Collections.Generic.List<string> Categories { get; set; }
    }
}
