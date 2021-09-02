using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
     public class TransactionController : Controller
     {
          public IActionResult Index()
          {
               return View();
          }
     }
}
