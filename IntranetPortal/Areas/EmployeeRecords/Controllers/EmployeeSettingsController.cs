using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.EmployeeRecords.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.EmployeeRecords.Controllers
{
    [Area("EmployeeRecords")]
    [Authorize]
    public class EmployeeSettingsController : Controller
    {
        [Authorize(Policy = "SystemUsersOnly", Roles = "ERMSETVWH")]
        public IActionResult Index()
        {
            return View();
        }
    }
}