using System;

namespace Interview.Web.Model
{
    public class Product
    {

        public int InstanceId { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
    }
}

