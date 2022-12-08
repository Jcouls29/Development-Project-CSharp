﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Models
{
    public partial class CategoryAttribute
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        //JsonIgnore prevents circular references when deserializing
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Category Instance { get; set; }
    }
}
