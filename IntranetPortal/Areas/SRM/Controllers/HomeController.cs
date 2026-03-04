using IntranetPortal.Areas.SRM.Models;
using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Controllers
{
    [Area("SRM")]
    public class HomeController : Controller
    {
        private readonly IRequestService _requestService;

        public HomeController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public async Task<IActionResult> ServiceSystems()
        {
            ServiceSystemsListViewModel model = new ServiceSystemsListViewModel();
            var entities = await _requestService.GetServiceSystemsAsync();
            if(entities != null) { model.ServiceSystemList = entities; }
            return View(model);
        }

        public async Task<IActionResult> ServiceCenters()
        {
            ServiceCentersListViewModel model = new ServiceCentersListViewModel();
            var entities = await _requestService.GetServiceCentersAsync();
            if (entities != null) { model.ServiceCenterList = entities; }
            return View(model);
        }


        #region Helper Controller Action Methods

        #region Service System Action Methods
        public string AddServiceSystem(string nm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm))
                {
                    return "parameter";
                }
                ServiceSystem system = new ServiceSystem()
                {
                    Name = nm,
                };

                if (_requestService.CreateServiceSystemAsync(system).Result > 0)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UpdateServiceSystem(int id, string nm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm) || id < 1)
                {
                    return "parameter";
                }


                ServiceSystem system = new ServiceSystem();
                system.Name = nm;
                system.Id = id;

                if (_requestService.UpdateServiceSystemAsync(system).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteServiceSystem(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_requestService.DeleteServiceSystemAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Service Center Action Methods
        public string AddServiceCenter(string nm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm))
                {
                    return "parameter";
                }
                ServiceCenter center = new ServiceCenter()
                {
                    Name = nm,
                };

                if (_requestService.CreateServiceCenterAsync(center).Result > 0)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UpdateServiceCenter(int id, string nm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm) || id < 1)
                {
                    return "parameter";
                }


                ServiceCenter center = new ServiceCenter();
                center.Name = nm;
                center.Id = id;

                if (_requestService.UpdateServiceCenterAsync(center).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteServiceCenter(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_requestService.DeleteServiceCenterAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
        
        #endregion
    }
}
