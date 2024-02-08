using System;

namespace Sparcpoint.SqlServer.Abstractions.Data
{
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
