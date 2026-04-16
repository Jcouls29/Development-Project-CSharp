using Sparcpoint.Models;
using System;
using System.Collections.Generic;

namespace Sparcpoint.Features.Products.Queries.GetById
{
    public class ProductGetByIdResponse
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }

        public DateTime CreatedTimestamp { get; set; }
    }
}
