using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models
{
    public class CatergoryAttributes
    {
        public Guid InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
