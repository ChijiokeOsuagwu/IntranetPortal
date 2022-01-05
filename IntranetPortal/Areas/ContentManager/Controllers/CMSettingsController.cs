using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    public class CMSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}