using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sparcpoint.Core.Entities
{
    [Table("[Instances].[CategoryAttributes]")]
    public class CategoryAttributes
    {
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}