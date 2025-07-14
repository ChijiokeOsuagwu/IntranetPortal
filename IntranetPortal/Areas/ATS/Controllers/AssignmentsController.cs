using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ATS.Models;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.ATS.Controllers
{
    [Area("ATS")]
    public class AssignmentsController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        public async Task<IActionResult> Active(string cn, DateTime? sd = null, DateTime? ed = null)
        {
            ActiveAssignmentsListViewModel model = new ActiveAssignmentsListViewModel();
            try
            {
                model.cn = cn;
                model.sd = sd ?? DateTime.Now.AddMonths(-1);
                model.ed = ed ?? DateTime.Now.AddMonths(1);
                model.AssignmentList = await _assignmentService.GetAssignments(model.cn, model.sd, model.ed);
            }
            catch(Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
    }
}