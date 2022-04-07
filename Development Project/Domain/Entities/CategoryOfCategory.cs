using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CategoryOfCategory
    {
        [Key]
        public int InstanceId { get; set; }

        public int CategoryInstanceId { get; set; }
    }
}
