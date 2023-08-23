using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Domain.Entities.Instances
{
    public abstract class InstanceEntity : Entity
    {
        protected override int Id { get; set; } = int.MinValue;
        public int InstanceId { get => Id; set => Id = value; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public IList<InstanceAttribute> Attributes { get; set; } = new List<InstanceAttribute>();
        public IList<Category> Categories { get; set; } = new List<Category>();
    }
}
