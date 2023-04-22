using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sparcpoint.Core.Models
{
    [Table("[Instances].[CategoryCategories]")]
    public class CategoryCategories
    {
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }
    }
}