using Sparkpoint.Data;

namespace Interview.Web.DTO
{
    public class ProductCreationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Product Product { get; set; }
    }
}
