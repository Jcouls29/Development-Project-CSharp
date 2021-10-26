using System.Collections.Generic;

namespace Interview.Data.ViewModels
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }

        public ICollection<GenericAttribute> ProductAttributes { get; set; }
        public ICollection<CategoryViewModel> Categories { get; set; }

    }
}
