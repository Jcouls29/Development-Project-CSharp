namespace Sparcpoint.Infrastructure.RequestModels
{
    public class CreateProductRequest
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ProductImageUris { get; set; }

        public string? ValidSkus { get; set; }
    }
}
