using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Requestes
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CategoryAttributeRequest> CategoryAttributes { get; set; }
        public List<CategoryOfCategoryRequest> Categories { get; set; }
    }
}
