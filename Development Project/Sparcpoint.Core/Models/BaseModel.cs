using System;

namespace Sparcpoint.Models
{
    // EVAL: Abstract class because it is not supposed to be instantiated
    public abstract class BaseModel
    {
        public int InstanceId { get; set; }

        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;

    }
}
