using IntranetPortal.Areas.SRM.Models;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Controllers
{
    [Area("SRM")]
    public class ServiceController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IGlobalSettingsService _globalSettingsService;
        

        public ServiceController(IRequestService requestService, IGlobalSettingsService globalSettingsService)
        {
            _requestService = requestService;
            _globalSettingsService = globalSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Service Request Controller Action Methods
        public async Task<IActionResult> NewRequest()
        {
            NewRequestViewModel model = new NewRequestViewModel();

            var service_teams_entities = await _globalSettingsService.GetTeamsAsync();
            if (service_teams_entities != null && service_teams_entities.Count > 0)
            {
                ViewBag.ServiceCentersList = new SelectList(service_teams_entities, "TeamID", "TeamName", model.ServiceCenterId);
            }

            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null && location_entities.Count > 0)
            {
                ViewBag.LocationsList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationId);
            }

            var service_system_entities = await _requestService.GetServiceSystemsAsync();
            if (service_system_entities != null && service_system_entities.Count > 0)
            {
                ViewBag.ServiceSystemsList = new SelectList(service_system_entities, "Id", "Name", model.ServiceSystemId);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewRequest(NewRequestViewModel model)
        {
            long newServiceIncidentId = 0;
            if (ModelState.IsValid)
            {
                model.ReportedByEmployeeName = HttpContext.User.Identity.Name;
                newServiceIncidentId = await _requestService.CreateServiceIncidentAsync(model.Convert());
                if(newServiceIncidentId > 0)
                {
                    model.ViewModelSuccessMessage = "New Service Request was sent successfully!";
                    model.OperationIsSuccessful = true;
                }
            }

            var service_center_entities = await _requestService.GetServiceCentersAsync();
            if (service_center_entities != null && service_center_entities.Count > 0)
            {
                ViewBag.ServiceCentersList = new SelectList(service_center_entities, "Id", "Name", model.ServiceCenterId);
            }

            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null && location_entities.Count > 0)
            {
                ViewBag.LocationsList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationId);
            }

            var service_system_entities = await _requestService.GetServiceSystemsAsync();
            if (service_system_entities != null && service_system_entities.Count > 0)
            {
                ViewBag.ServiceSystemsList = new SelectList(service_system_entities, "Id", "Name", model.ServiceSystemId);
            }

            return View(model);
        }
        public async Task<IActionResult> EditRequest(long id, string sp)
        {
            NewRequestViewModel model = new NewRequestViewModel();
            if(id < 1) { return RedirectToAction("NewRequest"); }
            var entity = await _requestService.GetServiceIncidentAsync(id);
            if(entity != null)
            {
                model = model.Convert(entity);
            }
            model.src = model.SourcePage = sp;

            var service_teams_entities = await _globalSettingsService.GetTeamsAsync();
            if (service_teams_entities != null && service_teams_entities.Count > 0)
            {
                ViewBag.ServiceCentersList = new SelectList(service_teams_entities, "TeamID", "TeamName", model.ServiceCenterId);
            }

            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null && location_entities.Count > 0)
            {
                ViewBag.LocationsList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationId);
            }

            var service_system_entities = await _requestService.GetServiceSystemsAsync();
            if (service_system_entities != null && service_system_entities.Count > 0)
            {
                ViewBag.ServiceSystemsList = new SelectList(service_system_entities, "Id", "Name", model.ServiceSystemId);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRequest(NewRequestViewModel model)
        {
            string updatedBy = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                ServiceIncident serviceIncident = model.Convert();

                bool IsUpdated = await _requestService.UpdateServiceIncidentAsync(serviceIncident, updatedBy);
                if (IsUpdated)
                {
                    model.ViewModelSuccessMessage = "Changes saved successfully!";
                    model.OperationIsSuccessful = true;
                }
            }

            var service_center_entities = await _requestService.GetServiceCentersAsync();
            if (service_center_entities != null && service_center_entities.Count > 0)
            {
                ViewBag.ServiceCentersList = new SelectList(service_center_entities, "Id", "Name", model.ServiceCenterId);
            }

            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null && location_entities.Count > 0)
            {
                ViewBag.LocationsList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationId);
            }

            var service_system_entities = await _requestService.GetServiceSystemsAsync();
            if (service_system_entities != null && service_system_entities.Count > 0)
            {
                ViewBag.ServiceSystemsList = new SelectList(service_system_entities, "Id", "Name", model.ServiceSystemId);
            }

            return View(model);
        }
        public async Task<IActionResult> MyServiceRequests(DateTime? sd = null, DateTime? ed = null)
        {
            MyServiceRequestsListViewModel model = new MyServiceRequestsListViewModel();
            var claims = HttpContext.User.Claims.ToList();
            model.RequestOwnerID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var entities = await _requestService.GetMyServiceIncidentsAsync(model.RequestOwnerID, sd, ed);
            if (entities != null) { model.ServiceIncidentsList = entities; }
            model.RequestOwnerName = HttpContext.User.Identity.Name;
            return View(model);
        }
        public async Task<IActionResult> ServiceRequestBoard(DateTime? sd = null, DateTime? ed = null)
        {
            MyServiceRequestsListViewModel model = new MyServiceRequestsListViewModel();
            model.TeamList = new List<Team>();
            var claims = HttpContext.User.Claims.ToList();
            model.RequestOwnerID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var entities = await _requestService.GetMyServiceIncidentsAsync(model.RequestOwnerID, sd, ed);
            if (entities != null) { model.ServiceIncidentsList = entities; }
            model.RequestOwnerName = HttpContext.User.Identity.Name;

            var teamEntities = await _globalSettingsService.GetTeamsAsync();
            if(teamEntities != null && teamEntities.Count > 0)
            {
                model.TeamList = teamEntities;
            }
            return View(model);
        }
        public async Task<IActionResult> RequestResolutions(long rd)
        {
            RequestResolutionsListViewModel model = new RequestResolutionsListViewModel();
            model.IncidentResolutionsList = new List<IncidentResolution>();
            model.rd = rd;
            model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
            ServiceIncident serviceIncident = new ServiceIncident();
            var service_incident_entity = await _requestService.GetServiceIncidentAsync(model.rd);
            if(service_incident_entity != null) { model.AssignedToEmployeeName = service_incident_entity.AssignedToName; }
            var entities = await _requestService.GetIncidentResolutionsAsync(model.rd);
            if (entities != null) { model.IncidentResolutionsList = entities; }

            return View(model);
        }
        public async Task<IActionResult> ManageRequestResolution(long id, long rd)
        {
            RequestResolutionViewModel model = new RequestResolutionViewModel();
            model.Id = id;
            model.IncidentId = rd;
            ServiceIncident incident = new ServiceIncident();
            try
            {
                incident = await _requestService.GetServiceIncidentAsync(model.IncidentId);
                if (incident != null)
                {
                    model.IncidentNumber = incident.Number;
                    model.IncidentDescription = incident.Description;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            var service_type_entities = await _requestService.GetServiceTypesAsync();
            if (service_type_entities != null && service_type_entities.Count > 0)
            {
                ViewBag.ServiceTypesList = new SelectList(service_type_entities, "Id", "Name", model.ServiceTypeId);
            }

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRequestResolution(RequestResolutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                string recordedBy = model.RecordedByEmployeeName = HttpContext.User.Identity.Name;

                try
                {
                    IncidentResolution incidentResolution = new IncidentResolution();
                    incidentResolution = model.Convert();
                    if(model.Id > 0)
                    {
                        bool isUpdated = await _requestService.UpdateIncidentResolutionAsync(incidentResolution, recordedBy);
                        if (isUpdated)
                        {
                            return RedirectToAction("RequestResolutions", new { rd = model.IncidentId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AssignRequest(long id, string td)
        {
            AssignRequestViewModel model = new AssignRequestViewModel();
            model.IncidentId = id;
            model.ServiceTeamId = td;
            var team_members_entities = await _globalSettingsService.GetTeamMembersByTeamIdAsync(model.ServiceTeamId);
            if (team_members_entities != null && team_members_entities.ToList().Count > 0)
            {
                ViewBag.TeamMembersList = new SelectList(team_members_entities, "FullName", "FullName", model.AssignedToEmployeeName);
            }
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRequest(AssignRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.AssignedByEmployeeName = HttpContext.User.Identity.Name;

                try
                {
                        bool isUpdated = await _requestService.UpdateIncidentAssignmentAsync(model.IncidentId, model.AssignedToEmployeeName, model.AssignedByEmployeeName);
                        if (isUpdated)
                        {
                            return RedirectToAction("ServiceRequestBoard");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var team_members_entities = await _globalSettingsService.GetTeamMembersByTeamIdAsync(model.ServiceTeamId);
            if (team_members_entities != null && team_members_entities.ToList().Count > 0)
            {
                ViewBag.TeamMembersList = new SelectList(team_members_entities, "FullName", "FullName", model.AssignedToEmployeeName);
            }
            return View(model);
        }

        #endregion

        #region Controller Helper Methods
        public string DeleteServiceIncident(int id)
        {
            if (id < 1) { return "parameter error"; }
            //string deletedBy = HttpContext.User.Identity.Name;
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
    }
}
