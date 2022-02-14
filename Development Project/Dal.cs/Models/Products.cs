﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Dal.Models
{
    public class Products
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreateTimestamp { get; set; }
    }
}
