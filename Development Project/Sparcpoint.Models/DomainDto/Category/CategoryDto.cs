using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DomainDto.Category
{
    public class CategoryDto
    {
        public Guid InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<CategoryAttributesDto> Attributes { get; set; }
        public virtual ICollection<CategoryOfCategoryDto> Categories { get; set; }
    }
}
