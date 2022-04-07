using System;
using System.Collections.Generic;
using System.Text;
using entity =  Domain.Entities;

namespace SparcpointServices.Interface
{
    interface ICategory
    {
        void Add(entity.Category category);
    }
}
