using IntranetPortal.Areas.LVM.Models;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Controllers
{
    [Area("LVM")]
    public class LeaveController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IBaseModelService _baseModelService;
        private readonly ILeaveService _leaveService;
        private readonly IErmService _ermService;

        public LeaveController(IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration, IBaseModelService baseModelService,
            ILeaveService leaveService, IErmService ermService)
        {
            _configuration = configuration;
            _baseModelService = baseModelService;
            _leaveService = leaveService;
            _ermService = ermService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MyLeavePlans(int yr)
        {
            MyLeavePlansListViewModel model = new MyLeavePlansListViewModel();
            if (yr < 2020)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }

            model.nm = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                //model.LeavePlanList = await _leaveService.GetEmployeeLeavesAsync(model.ei, model.yr, true);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

    }
}
