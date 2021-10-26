using System.Collections.Generic;

namespace Interview.Data.ViewModels
{
    public class CategoryViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<GenericAttribute> CategoryAttributes { get; set; }
    }
}