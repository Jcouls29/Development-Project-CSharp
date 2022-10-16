using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public abstract class MetadataEntity
    {
        public virtual int InstanceId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedTimestamp { get; set; }
        public virtual Dictionary<string, string> Metadata { get; set; }
    }
}
