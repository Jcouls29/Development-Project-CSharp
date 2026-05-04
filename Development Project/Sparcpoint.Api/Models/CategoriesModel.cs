using System;

namespace Sparcpoint.API.Models
{
    public class CategoriesModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
