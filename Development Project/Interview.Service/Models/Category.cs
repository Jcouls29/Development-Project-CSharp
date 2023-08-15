using System;
using System.Collections.Generic;

namespace Interview.Service.Models
{
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public List<CustAttribute> CategoryAttributes { get; set; }
    }
}