using System;
using System.Collections.Generic;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Domain.Dto;

public class ProductData
{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Unit Unit { get; set; }
        public ICollection<MetadataData> Metadatas { get; set; }
        public ICollection<CategoryData> Categories { get; set; }
}