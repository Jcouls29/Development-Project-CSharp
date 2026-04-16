using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Category
    {
        public int InstanceId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedTimestamp { get; private set; }
    }
}
