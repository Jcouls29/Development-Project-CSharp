using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Domain.Entities
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        
        [StringLength(FieldLength.Name)]
        public string Name { get; set; }
        
        [StringLength(FieldLength.Description)]
        public string Description { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        
        public ICollection<Metadata> Metadatas { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}