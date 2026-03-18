using IntranetPortal.Areas.SRM.Models;
using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authentication;
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

        public async Task<IActionResult> Notes(string sp = null, long? id = null)
        {
            ServiceRequestNotesViewModel model = new ServiceRequestNotesViewModel();
            model.ServiceIncidentId = id;
            model.SourcePage = model.src = sp;

            if (model.ServiceIncidentId == null || model.ServiceIncidentId < 1) { return View(model); }
            else
            {
                    var requestNotes = await _requestService.GetServiceRequestNotesAsync(model.ServiceIncidentId.Value);
                    if (requestNotes != null) { model.NoteList = requestNotes; }
            }

            model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
            model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

            if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }
            return View(model);
        }
        
        
        public async Task<IActionResult> Activities(long? id)
        {
            ServiceRequestActivitiesViewModel model = new ServiceRequestActivitiesViewModel();

            model.ServiceIncidentId = id;
            if (id == null || id < 1) { return View(model); }

            if (model.ServiceIncidentId > 0)
            {
                var requestActivities = await _requestService.GetServiceRequestActivitiesAsync(model.ServiceIncidentId.Value);
                if (requestActivities != null) { model.ActivityList = requestActivities; }
            }
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

        #region Notes Action Methods
        public string SaveRequestNote(string nm, string msg, long id)
        {
            ServiceRequestNote note = new ServiceRequestNote()
            {
                NoteTime = DateTime.Now,
                NoteWrittenBy = nm,
                NoteContent = msg,
                ServiceIncidentId = id,
            };

            if (note.ServiceIncidentId < 1  || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            try
            {
                if (_requestService.AddServiceRequestNoteAsync(note).Result)
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
        #endregion

        #region Service Incidents Helper Action Methods
        public string UpdateIncidentStatus(long id, string ns, string os)
        {
            if (id < 1) { return "parameter error"; }
            string updatedBy = HttpContext.User.Identity.Name;
            try
            {
                if (_requestService.UpdateServiceIncidentStatusAsync(id, os, ns, updatedBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }

        public string DeleteServiceIncident(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_requestService.DeleteServiceIncidentAsync(id).Result)
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
