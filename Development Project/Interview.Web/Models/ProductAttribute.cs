﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Models
{
    public partial class ProductAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Product Instance { get; set; }
    }
}
