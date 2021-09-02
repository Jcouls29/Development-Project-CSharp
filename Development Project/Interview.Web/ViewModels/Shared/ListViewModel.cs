using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.ViewModels.Shared
{
     public class ListViewModel<T>
     {
          public List<T> Items { get; set; }
     }
}
