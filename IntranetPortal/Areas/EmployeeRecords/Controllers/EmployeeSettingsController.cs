using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.EmployeeRecords.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.EmployeeRecords.Controllers
{
    [Area("EmployeeRecords")]
    public class EmployeeSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}