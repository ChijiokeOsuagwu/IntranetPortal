using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.PartnerServices.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.PartnerServices.Controllers
{
    [Area("PartnerServices")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IBaseModelService _baseModelService;
        public HomeController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBusinessManagerService businessManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _businessManagerService = businessManagerService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}