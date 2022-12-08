using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Models
{
    public partial class CategoryCategory
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Category CategoryInstance { get; set; }
        //JsonIgnore prevents circular references when deserializing
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Category Instance { get; set; }
    }
}
