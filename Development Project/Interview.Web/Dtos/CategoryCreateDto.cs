using System;

namespace Interview.Web.Dtos
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
}
