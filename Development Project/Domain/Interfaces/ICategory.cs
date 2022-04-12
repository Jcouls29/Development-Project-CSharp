using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface ICategory
    {

        string ProductName { get; set; }
        string ProductDescription { get; set; }
        DateTime CreatedTimestamp { get; }
    }
}