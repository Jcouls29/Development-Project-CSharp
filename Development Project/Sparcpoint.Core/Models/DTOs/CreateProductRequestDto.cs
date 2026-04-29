using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DTOs
{
    public class CreateProductRequestDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUris { get; set; }

        public string Skus { get; set; }

        public List<int> CategoryIds { get; set; }

        //EVAL: Consistent pre-defined extra attributes for each product instead of adding metada dynamically each time
        public string AdditionalSku { get; set; }
        public string Color { get; set; }

        public decimal? Length { get; set; }

        public string PackageUnit { get; set; }
    }
}
