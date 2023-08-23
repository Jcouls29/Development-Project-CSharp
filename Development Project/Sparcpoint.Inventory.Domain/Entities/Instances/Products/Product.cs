using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparcpoint.Inventory.Domain.Entities.Instances
{
    public class Product : InstanceEntity
    {
        private const string _DefaultTagSeparator = ",";

        public string ImageUris { get; set; } = string.Empty;
        public string ValidSkus { get; set; } = string.Empty;

        public IEnumerable<string> ImageUrisList
        {
            get => string.IsNullOrWhiteSpace(ImageUris) ?
                   Enumerable.Empty<string>() :
                   ImageUris.Split(new[] { _DefaultTagSeparator }, StringSplitOptions.RemoveEmptyEntries);
            set => ImageUris = string.Join(_DefaultTagSeparator, value);
        }
        public IEnumerable<string> ValidSkusList
        {
            get => string.IsNullOrWhiteSpace(ValidSkus) ?
                   Enumerable.Empty<string>() :
                   ValidSkus.Split(new[] { _DefaultTagSeparator }, StringSplitOptions.RemoveEmptyEntries);
            set => ValidSkus = string.Join(_DefaultTagSeparator, value);
        }

    }
}
