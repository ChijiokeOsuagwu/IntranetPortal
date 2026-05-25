using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.SrmRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class RequestService : IRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        //private readonly IUtilityRepository _utilityRepository;
        //private readonly IProgramRepository _programRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ITeamRepository _teamRepository;

        public RequestService(IServiceRequestRepository serviceRequestRepository, IEmployeesRepository employeesRepository,
            ITeamRepository teamRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _employeesRepository = employeesRepository;
            _teamRepository = teamRepository;
        }

        #region Service Incident Action Methods
        #region Write Action Methods
        public async Task<long> CreateServiceIncidentAsync(ServiceIncident serviceIncident)
        {
            long _newId = 0;
            if (serviceIncident == null) { throw new ArgumentNullException(nameof(serviceIncident), "The required parameter [Service Incident] is missing."); }
            string newCodeNumber = await GetIncidentCodeNumber();
            if (string.IsNullOrWhiteSpace(newCodeNumber)) { throw new Exception("Error: Invalid Code Number."); }
            serviceIncident.Number = newCodeNumber;
            serviceIncident.IncidentStatus = "Pending";
            serviceIncident.IsFalseNegative = false;
            serviceIncident.IsAssigned = false;
            
            if (!string.IsNullOrWhiteSpace(serviceIncident.IncidentEmployeeName))
            {
                var incidentEmployee = await _employeesRepository.GetEmployeeByNameAsync(serviceIncident.IncidentEmployeeName);
                if(incidentEmployee != null && !string.IsNullOrWhiteSpace(incidentEmployee.EmployeeID))
                {
                    serviceIncident.IncidentEmployeeId = incidentEmployee.EmployeeID;
                    serviceIncident.UnitId = incidentEmployee.UnitID;
                    serviceIncident.DepartmentId = incidentEmployee.DepartmentID;
                }
            }
            _newId = await _serviceRequestRepository.AddServiceIncidentAsync(serviceIncident);
            if(_newId > 0)
            {
                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = serviceIncident.ReportedByEmployeeName,
                    ActivityDescription = $"New Service Request was created by {serviceIncident.ReportedByEmployeeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.",
                    ServiceIncidentId = _newId,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return _newId;
        }
        public async Task<bool> UpdateServiceIncidentAsync(ServiceIncident serviceIncident, string updatedBy)
        {
            if (serviceIncident == null) { throw new ArgumentNullException(nameof(serviceIncident), "The required parameter [Service Request] is missing."); }
            ServiceIncident oldServiceIncident = new ServiceIncident();
            List<ServiceIncident> oldServiceIncidentList = await _serviceRequestRepository.GetServiceIncidentByIdAsync(serviceIncident.Id);
            if(oldServiceIncidentList != null && oldServiceIncidentList.Count == 1)
            {
                oldServiceIncident = oldServiceIncidentList.FirstOrDefault();
                serviceIncident.IncidentStatus = oldServiceIncident.IncidentStatus;
            }

            if (!string.IsNullOrWhiteSpace(serviceIncident.IncidentEmployeeName))
            {
                var incidentEmployee = await _employeesRepository.GetEmployeeByNameAsync(serviceIncident.IncidentEmployeeName);
                if (incidentEmployee != null && !string.IsNullOrWhiteSpace(incidentEmployee.EmployeeID))
                {
                    serviceIncident.IncidentEmployeeId = incidentEmployee.EmployeeID;
                    serviceIncident.UnitId = incidentEmployee.UnitID;
                    serviceIncident.DepartmentId = incidentEmployee.DepartmentID;
                }
            }

            bool IsUpdated = await _serviceRequestRepository.UpdateServiceIncidentAsync(serviceIncident);
            if (IsUpdated)
            {
                StringBuilder sb = new StringBuilder();

                if (serviceIncident.Description != oldServiceIncident.Description)
                {
                    sb.AppendLine($"Changed Problem Description from: [{oldServiceIncident.Description}] to [{serviceIncident.Description}].");
                }

                if (!(string.IsNullOrWhiteSpace(serviceIncident.Impact) && string.IsNullOrWhiteSpace(oldServiceIncident.Impact)))
                {
                    if (serviceIncident.Impact != oldServiceIncident.Impact)
                    {
                        sb.AppendLine($"Changed Problem Impact from: [{oldServiceIncident.Impact}] to [{serviceIncident.Impact}].");
                    }
                }

                if (!(serviceIncident.IncidentTime == null && oldServiceIncident.IncidentTime == null))
                {
                    if (serviceIncident.IncidentTime != oldServiceIncident.IncidentTime)
                    {
                        sb.AppendLine($"Changed Incident Date from: [{oldServiceIncident.IncidentTime}] to [{serviceIncident.IncidentTime}].");
                    }
                }

                if (!(string.IsNullOrWhiteSpace(serviceIncident.ServiceCenterId) && string.IsNullOrWhiteSpace(oldServiceIncident.ServiceCenterId)))
                {
                    if (serviceIncident.ServiceCenterId != oldServiceIncident.ServiceCenterId)
                    {
                        var newServiceCenter = await _teamRepository.GetTeamByIdAsync(serviceIncident.ServiceCenterId);

                        sb.AppendLine($"Changed Service Center from: [{oldServiceIncident?.ServiceCenterName}] to [{newServiceCenter?.TeamName}].");
                    }
                }

                    if (serviceIncident.IncidentEmployeeId != oldServiceIncident.IncidentEmployeeId)
                    {
                    var newIncidentEmployee = await _employeesRepository.GetEmployeeByIdAsync(serviceIncident.IncidentEmployeeId);
                        sb.AppendLine($"Changed the Incident Employee from: [{oldServiceIncident.IncidentEmployeeName}] to [{newIncidentEmployee?.FullName}].");
                    }

                if (serviceIncident.Severity != oldServiceIncident.Severity)
                {
                    string _severityDescription = string.Empty;
                    switch (serviceIncident.Severity)
                    {
                        case 0:
                            _severityDescription = "Low";
                            break;
                        case 1:
                            _severityDescription = "Medium";
                            break;
                        case 2:
                            _severityDescription = "High";
                            break;
                        case 3:
                            _severityDescription = "Critical";
                            break;
                        default:
                            break;
                    }
                    sb.AppendLine($"Changed the Incident Severity from: [{oldServiceIncident.SeverityDescription}] to [{_severityDescription}].");
                }

                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = updatedBy,
                    ActivityDescription = sb.ToString(),
                    ServiceIncidentId = serviceIncident.Id,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return IsUpdated;
        }

        public async Task<bool> UpdateServiceIncidentStatusAsync(long serviceIncidentId, string oldIncidentStatus, string newIncidentStatus, string updatedBy)
        {
            if (serviceIncidentId < 1) { throw new ArgumentNullException(nameof(serviceIncidentId), "The required parameter [Service Request ID] is missing."); }
            if (string.IsNullOrWhiteSpace(newIncidentStatus)) { throw new ArgumentNullException(nameof(serviceIncidentId), "The required parameter [New Incident Status] is missing."); }


            bool IsUpdated = await _serviceRequestRepository.UpdateServiceIncidentStatusAsync(serviceIncidentId, newIncidentStatus);
            if (IsUpdated)
            {
                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = updatedBy,
                    ActivityDescription = $"Incident Status was changed from {oldIncidentStatus} to {newIncidentStatus} by {updatedBy} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}.",
                    ServiceIncidentId = serviceIncidentId,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return IsUpdated;
        }

        public async Task<bool> DeleteServiceIncidentAsync(long serviceIncidentId)
        {
            if (serviceIncidentId < 1) { throw new ArgumentNullException(nameof(serviceIncidentId), "The required parameter [Service Incident ID] is missing."); }
            return await _serviceRequestRepository.DeleteServiceIncidentAsync(serviceIncidentId);
        }

        #endregion

        #region Read Action Methods
        public async Task<List<ServiceIncident>> GetMyServiceIncidentsAsync(string TaskOwnerId, DateTime? StartDate, DateTime? EndDate)
        {
            List<ServiceIncident> serviceIncidents = new List<ServiceIncident>();
            var entities = await _serviceRequestRepository.GetServiceIncidentsByOwnerIdAsync(TaskOwnerId, StartDate, EndDate);
            if (entities != null)
            {
                serviceIncidents = entities;
            }
            return serviceIncidents;
        }
        public async Task<List<ServiceIncident>> GetMyTeamsServiceIncidentsAsync(string TeamMemberId, DateTime? StartDate, DateTime? EndDate)
        {
            List<ServiceIncident> serviceIncidents = new List<ServiceIncident>();
            if (!string.IsNullOrWhiteSpace(TeamMemberId))
            {
                var entities = await _serviceRequestRepository.GetServiceIncidentsByTeamMemberIdAsync(TeamMemberId, StartDate, EndDate);
                if (entities != null)
                {
                    serviceIncidents = entities;
                }
            }
            return serviceIncidents;
        }


        public async Task<ServiceIncident> GetServiceIncidentAsync(long ServiceIncidentId)
        {
            ServiceIncident serviceIncident = new ServiceIncident();
            var entities = await _serviceRequestRepository.GetServiceIncidentByIdAsync(ServiceIncidentId);
            if (entities != null && entities.Count > 0)
            {
                serviceIncident = entities.FirstOrDefault();
            }
            return serviceIncident;
        }

        #endregion

        #endregion

        #region Incident Resolutions Action Methods
        #region Read Action Methods
        public async Task<List<IncidentResolution>> GetIncidentResolutionsAsync(long ServiceIncidentId)
        {
            List<IncidentResolution> incidentResolutions = new List<IncidentResolution>();
            var entities = await _serviceRequestRepository.GetIncidentResolutionsByIncidentIdAsync(ServiceIncidentId);
            if (entities != null)
            {
                incidentResolutions = entities;
            }
            return incidentResolutions;
        }
        public async Task<IncidentResolution> GetIncidentResolutionAsync(long IncidentResolutionId)
        {
            IncidentResolution incidentResolution = new IncidentResolution();
            var entities = await _serviceRequestRepository.GetIncidentResolutionsByIdAsync(IncidentResolutionId);
            if (entities != null && entities.Count > 0)
            {
                incidentResolution = entities.FirstOrDefault();
            }
            return incidentResolution;
        }

        #endregion

        #region Write Action Methods
        public async Task<bool> AddIncidentResolutionAsync(IncidentResolution incidentResolution)
        {
            long _newId = 0;
            if (incidentResolution == null) { throw new ArgumentNullException(nameof(incidentResolution), "The required parameter [Incident Resolution] is missing."); }

            if (!string.IsNullOrWhiteSpace(incidentResolution.ResolvedByEmployeeName))
            {
                var solutionEmployee = await _employeesRepository.GetEmployeeByNameAsync(incidentResolution.ResolvedByEmployeeName);
                if (solutionEmployee != null && !string.IsNullOrWhiteSpace(solutionEmployee.EmployeeID))
                {
                    incidentResolution.ResolvedByEmployeeId = solutionEmployee.EmployeeID;
                }
            }
            _newId = await _serviceRequestRepository.AddIncidentResolutionAsync(incidentResolution);
            if (_newId > 0)
            {
                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = incidentResolution.RecordedByEmployeeName,
                    ActivityDescription = $"New Request Resolution was added by {incidentResolution.RecordedByEmployeeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}.",
                    ServiceIncidentId = incidentResolution.IncidentId,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return _newId > 0;
        }
        public async Task<bool> UpdateIncidentResolutionAsync(IncidentResolution incidentResolution, string updatedBy)
        {
            if (incidentResolution == null) { throw new ArgumentNullException(nameof(incidentResolution), "The required parameter [Service Request] is missing."); }
            IncidentResolution oldSolution = new IncidentResolution();
            List<IncidentResolution> oldResolutionList = await _serviceRequestRepository.GetIncidentResolutionsByIdAsync(incidentResolution.Id);
            if (oldResolutionList != null && oldResolutionList.Count == 1)
            {
                oldSolution = oldResolutionList.FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(incidentResolution.ResolvedByEmployeeName))
            {
                var solutionEmployee = await _employeesRepository.GetEmployeeByNameAsync(incidentResolution.ResolvedByEmployeeName);
                if (solutionEmployee != null && !string.IsNullOrWhiteSpace(solutionEmployee.EmployeeID))
                {
                    incidentResolution.ResolvedByEmployeeId = solutionEmployee.EmployeeID;
                }
            }

            bool IsUpdated = await _serviceRequestRepository.UpdateIncidentResolutionAsync(incidentResolution);
            if (IsUpdated)
            {
                StringBuilder sb = new StringBuilder();

                if (incidentResolution.ResolutionDescription != oldSolution.ResolutionDescription)
                {
                    sb.AppendLine($"Changed Solution Description from: [{oldSolution.ResolutionDescription}] to [{incidentResolution.ResolutionDescription}].");
                }

                if (!(string.IsNullOrWhiteSpace(incidentResolution.ResolvedByEmployeeId) && string.IsNullOrWhiteSpace(oldSolution.ResolvedByEmployeeId)))
                {
                    if (incidentResolution.ResolvedByEmployeeId != oldSolution.ResolvedByEmployeeId)
                    {
                        sb.AppendLine($"Changed Resolved By from: [{oldSolution.ResolvedByEmployeeName}] to [{incidentResolution.ResolvedByEmployeeName}].");
                    }
                }

                if (!(incidentResolution.ResolvedTime == null && oldSolution.ResolvedTime == null))
                {
                    if (incidentResolution.ResolvedTime != oldSolution.ResolvedTime)
                    {
                        sb.AppendLine($"Changed Resolved On from: [{oldSolution.ResolvedTime?.ToString("f")}] to [{incidentResolution.ResolvedTime?.ToString("f")}].");
                    }
                }

                if (!(incidentResolution.ServiceTypeId < 1 && oldSolution.ServiceTypeId < 1))
                {
                    if (incidentResolution.ServiceTypeId != oldSolution.ServiceTypeId)
                    {
                        var newServiceType = await _serviceRequestRepository.GetServiceTypeByIdAsync(incidentResolution.ServiceTypeId.Value);

                        sb.AppendLine($"Changed Service Type from: [{oldSolution?.ServiceTypeName}] to [{newServiceType?.Name}].");
                    }
                }

                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = updatedBy,
                    ActivityDescription = sb.ToString(),
                    ServiceIncidentId = incidentResolution.IncidentId,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> DeleteIncidentResolutionAsync(long incidentResolutionId)
        {
            if (incidentResolutionId < 1) { throw new ArgumentNullException(nameof(incidentResolutionId), "The required parameter [Incident Resolution ID] is missing."); }
            return await _serviceRequestRepository.DeleteIncidentResolutionAsync(incidentResolutionId);
        }


        public async Task<bool> UpdateIncidentAssignmentAsync(long ServiceIncidentId, string AssignedToEmployeeName, string AssignedByEmployeeName)
        {
            if (ServiceIncidentId < 1) { throw new ArgumentNullException(nameof(ServiceIncidentId), "The required parameter [Service Incident ID] is missing."); }
            ServiceIncident oldIncident = new ServiceIncident();
            List<ServiceIncident> oldIncidentsList = await _serviceRequestRepository.GetServiceIncidentByIdAsync(ServiceIncidentId);
            if (oldIncidentsList != null && oldIncidentsList.Count == 1)
            {
                oldIncident = oldIncidentsList.FirstOrDefault();
            }

            bool IsUpdated = await _serviceRequestRepository.UpdateServiceIncidentAssignmentAsync(ServiceIncidentId, AssignedToEmployeeName);
            if (IsUpdated)
            {
                StringBuilder sb = new StringBuilder();

                if (AssignedToEmployeeName != oldIncident.AssignedToName)
                {
                    if (string.IsNullOrWhiteSpace(oldIncident.AssignedToName))
                    {
                        sb.AppendLine($"Request was assigned to [{AssignedToEmployeeName}] by [{AssignedByEmployeeName}] on {DateTime.Now.ToLongDateString()} at exactly {DateTime.Now.ToLongTimeString()}.");
                    }
                    else
                    {
                        sb.AppendLine($"Request was re-assigned from [{oldIncident.AssignedToName}] to [{AssignedToEmployeeName}] by [{AssignedByEmployeeName}] on {DateTime.Now.ToLongDateString()} at exactly {DateTime.Now.ToLongTimeString()}.");

                    }
                }

                ServiceRequestActivity activityLog = new ServiceRequestActivity
                {
                    ActivityTime = DateTime.Now,
                    ActivityBy = AssignedByEmployeeName,
                    ActivityDescription = sb.ToString(),
                    ServiceIncidentId = ServiceIncidentId,
                };
                await _serviceRequestRepository.AddServiceRequestActivityAsync(activityLog);
            }
            return IsUpdated;
        }

        #endregion

        #endregion

        #region Service Systems Action Methods
        public async Task<long> CreateServiceSystemAsync(ServiceSystem serviceSystem)
        {
            if (serviceSystem == null) { throw new ArgumentNullException(nameof(serviceSystem), "The required parameter [Service System] is missing."); }
            var entities = await _serviceRequestRepository.GetServiceSystemsByNameAsync(serviceSystem.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.AddServiceSystemAsync(serviceSystem);
        }
        public async Task<bool> UpdateServiceSystemAsync(ServiceSystem serviceSystem)
        {
            if (serviceSystem == null) { throw new ArgumentNullException(nameof(serviceSystem), "The required parameter [Service System] is missing."); }
            var entities = await _serviceRequestRepository.GetServiceSystemsByNameAsync(serviceSystem.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.UpdateServiceSystemAsync(serviceSystem);
        }
        public async Task<bool> DeleteServiceSystemAsync(int serviceSystemId)
        {
            return await _serviceRequestRepository.DeleteServiceSystemAsync(serviceSystemId);
        }
        public async Task<ServiceSystem> GetServiceSystemAsync(int serviceSystemId)
        {
            ServiceSystem serviceSystem = new ServiceSystem();
            var entities = await _serviceRequestRepository.GetServiceSystemByIdAsync(serviceSystemId);
            if (entities != null && entities.Count > 0) { serviceSystem = entities.FirstOrDefault(); }
            return serviceSystem;
        }
        public async Task<List<ServiceSystem>> GetServiceSystemsAsync()
        {
            List<ServiceSystem> serviceSystems = new List<ServiceSystem>();
            var entities = await _serviceRequestRepository.GetServiceSystemsAsync();
            if (entities != null && entities.Count > 0) { serviceSystems = entities; }
            return serviceSystems;
        }
        #endregion

        #region Service Centers Action Methods
        public async Task<long> CreateServiceCenterAsync(ServiceCenter serviceCenter)
        {
            if (serviceCenter == null) { throw new ArgumentNullException(nameof(serviceCenter), "The required parameter [Service System] is missing."); }
            var entities = await _serviceRequestRepository.GetServiceCentersByNameAsync(serviceCenter.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.AddServiceCenterAsync(serviceCenter);
        }
        public async Task<bool> UpdateServiceCenterAsync(ServiceCenter serviceCenter)
        {
            if (serviceCenter == null) { throw new ArgumentNullException(nameof(serviceCenter), "The required parameter [Service Center] is missing."); }
            var entities = await _serviceRequestRepository.GetServiceCentersByNameAsync(serviceCenter.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.UpdateServiceCenterAsync(serviceCenter);
        }
        public async Task<bool> DeleteServiceCenterAsync(int serviceCenterId)
        {
            return await _serviceRequestRepository.DeleteServiceCenterAsync(serviceCenterId);
        }
        public async Task<ServiceCenter> GetServiceCenterAsync(int serviceCenterId)
        {
            ServiceCenter serviceCenter = new ServiceCenter();
            var entities = await _serviceRequestRepository.GetServiceCenterByIdAsync(serviceCenterId);
            if (entities != null && entities.Count > 0) { serviceCenter = entities.FirstOrDefault(); }
            return serviceCenter;
        }
        public async Task<List<ServiceCenter>> GetServiceCentersAsync()
        {
            List<ServiceCenter> serviceCenters = new List<ServiceCenter>();
            var entities = await _serviceRequestRepository.GetServiceCentersAsync();
            if (entities != null && entities.Count > 0) { serviceCenters = entities; }
            return serviceCenters;
        }
        #endregion

        #region Service Types Action Methods
        public async Task<long> CreateServiceTypeAsync(ServiceType serviceType)
        {
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType), "The required parameter [Service System] is missing."); }
            var entity = await _serviceRequestRepository.GetServiceTypesByNameAsync(serviceType.Name);
            if (entity != null && entity.Id > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.AddServiceTypeAsync(serviceType);
        }
        public async Task<bool> UpdateServiceTypeAsync(ServiceType serviceType)
        {
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType), "The required parameter [Service Type] is missing."); }
            var entity = await _serviceRequestRepository.GetServiceTypesByNameAsync(serviceType.Name);
            if (entity != null && entity.Id > 0)
            {
                throw new Exception("Duplicate entry. This name already exists. Please choose another name.");
            }
            return await _serviceRequestRepository.UpdateServiceTypeAsync(serviceType);
        }
        public async Task<bool> DeleteServiceTypeAsync(int serviceTypeId)
        {
            return await _serviceRequestRepository.DeleteServiceTypeAsync(serviceTypeId);
        }
        public async Task<ServiceType> GetServiceTypeAsync(int serviceTypeId)
        {
            ServiceType serviceType = new ServiceType();
            var entity = await _serviceRequestRepository.GetServiceTypeByIdAsync(serviceTypeId);
            if (entity != null && entity.Id > 0) { serviceType = entity; }
            return serviceType;
        }
        public async Task<List<ServiceType>> GetServiceTypesAsync()
        {
            List<ServiceType> serviceTypes = new List<ServiceType>();
            var entities = await _serviceRequestRepository.GetServiceTypesAsync();
            if (entities != null && entities.Count > 0) { serviceTypes = entities; }
            return serviceTypes;
        }
        #endregion

        #region Service Incident Notes and Activity Logs Service Actions
        public async Task<List<ServiceRequestActivity>> GetServiceRequestActivitiesAsync(long serviceIncidentId)
        {
            List<ServiceRequestActivity> activityLogs = new List<ServiceRequestActivity>();
            var entities = await _serviceRequestRepository.GetServiceRequestActivitysByServiceIncidentIdAsync(serviceIncidentId);
            if (entities != null && entities.Count > 0) { activityLogs = entities.ToList(); }
            return activityLogs;
        }
        public async Task<bool> AddServiceRequestNoteAsync(ServiceRequestNote serviceRequestNote)
        {
            if (serviceRequestNote == null) { throw new ArgumentNullException(nameof(serviceRequestNote), "The required parameter [Note] is missing."); }
            return await _serviceRequestRepository.AddNoteAsync(serviceRequestNote);
        }
        public async Task<List<ServiceRequestNote>> GetServiceRequestNotesAsync(long serviceIncidentId)
        {
            List<ServiceRequestNote> requestNotes = new List<ServiceRequestNote>();
            var entities = await _serviceRequestRepository.GetServiceRequestNotesByIncidentIdAsync(serviceIncidentId);
            if (entities != null && entities.Count > 0) { requestNotes = entities.ToList(); }
            return requestNotes;
        }
        #endregion

        #region Utility Helper Action Methods
        public async Task<string> GetIncidentCodeNumber()
        {
            char FirstCharacter = 'R';
            List<string> _existingNumbers = new List<string>();
            string yy = DateTime.Now.Year.ToString().Substring(2, 2);
            string monthCode = string.Empty;
            switch (DateTime.Now.Month)
            {
                case 1:
                    monthCode = "J";
                    break;
                case 2:
                    monthCode = "F";
                    break;
                case 3:
                    monthCode = "M";
                    break;
                case 4:
                    monthCode = "P";
                    break;
                case 6:
                    monthCode = "N";
                    break;
                case 7:
                    monthCode = "L";
                    break;
                case 8:
                    monthCode = "G";
                    break;
                case 9:
                    monthCode = "S";
                    break;
                case 10:
                    monthCode = "C";
                    break;
                case 11:
                    monthCode = "V";
                    break;
                case 12:
                    monthCode = "D";
                    break;
                default:
                    break;
            }

            _existingNumbers = await _serviceRequestRepository.GetIncidentCodeNumbersByCreatedDateAsync(DateTime.Now);
            if (_existingNumbers == null || _existingNumbers.Count < 1)
            {
                return $"{FirstCharacter}{yy}{monthCode}0001";
            }

            string _newAssignmentNumber = string.Empty;
            int _nextCount = 1;
            bool _isExisting = true;
            do
            {
                string _nextDigitString = _nextCount.ToString().PadLeft(4, '0');
                _newAssignmentNumber = $"{FirstCharacter}{yy}{monthCode}{_nextDigitString}";
                _isExisting = _existingNumbers.Contains(_newAssignmentNumber);
                _nextCount++;
            }
            while (_isExisting);
            return _newAssignmentNumber;
        }

        #endregion
    }
}
