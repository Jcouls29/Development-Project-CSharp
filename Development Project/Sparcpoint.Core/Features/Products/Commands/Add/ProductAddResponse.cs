using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Features.Products.Commands.Add
{
    public class ProductAddResponse
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }

        public DateTime CreatedTimestamp {  get; set; }

        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; }

        public List<Models.AttributeItem> Attributes { get; set; } = new List<Models.AttributeItem>();
    }
}
