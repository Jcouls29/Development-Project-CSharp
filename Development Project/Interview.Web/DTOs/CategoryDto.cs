using System;

namespace Interview.Web.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
