using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.CLM.Controllers
{
    [Area("CLM")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}