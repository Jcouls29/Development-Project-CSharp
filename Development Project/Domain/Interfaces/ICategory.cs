using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface ICategory
    {

         string Name { get; set; }
         string Description { get; set; }
         DateTime CreatedTimestamp { get; }
    }
}
