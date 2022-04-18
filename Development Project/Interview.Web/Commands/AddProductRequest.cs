using System;
using System.Collections.Generic;
using Interview.Web.Domain.Entities;

namespace Interview.Web.Commands;

//TODO: Needed ?
public class AddProductRequest
{
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        
        public ICollection<Metadata> Metadatas { get; set; }
        public ICollection<Category> Categories { get; set; }
}