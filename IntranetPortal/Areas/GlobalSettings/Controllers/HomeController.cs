using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.GlobalSettings.Controllers
{
    [Area("GlobalSettings")]
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize(Roles = "GBSVWHMPG, XYALLACCZ")]
        public IActionResult Index()
        {
            return View();
        }
    }
}