using IntranetPortal.Base.Models.SrmModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IRequestService
    {
        #region Service Incidents Action Interfaces
        Task<long> CreateServiceIncidentAsync(ServiceIncident serviceIncident);
        Task<bool> UpdateServiceIncidentAsync(ServiceIncident serviceIncident, string updatedBy);
        Task<bool> UpdateServiceIncidentStatusAsync(long serviceIncidentId, string oldIncidentStatus, string newIncidentStatus, string updatedBy);
        Task<bool> UpdateIncidentAssignmentAsync(long ServiceIncidentId, string AssignedToEmployeeName, string AssignedByEmployeeName);

        Task<bool> DeleteServiceIncidentAsync(long serviceIncidentId);

        Task<ServiceIncident> GetServiceIncidentAsync(long ServiceIncidentId);
        Task<List<ServiceIncident>> GetMyServiceIncidentsAsync(string TaskOwnerId, DateTime? StartDate, DateTime? EndDate);
        Task<List<ServiceIncident>> GetMyTeamsServiceIncidentsAsync(string TeamMemberId, DateTime? StartDate, DateTime? EndDate);
        #endregion

        #region Incident Resolution Action Interfaces
        Task<List<IncidentResolution>> GetIncidentResolutionsAsync(long ServiceIncidentId);
        Task<IncidentResolution> GetIncidentResolutionAsync(long IncidentResolutionId);

        Task<bool> AddIncidentResolutionAsync(IncidentResolution incidentResolution);
        Task<bool> UpdateIncidentResolutionAsync(IncidentResolution incidentResolution, string updatedBy);
        Task<bool> DeleteIncidentResolutionAsync(long incidentResolutionId);
        #endregion

        #region Settings Action Interfaces
        Task<long> CreateServiceCenterAsync(ServiceCenter serviceCenter);
        Task<long> CreateServiceSystemAsync(ServiceSystem serviceSystem);
        Task<long> CreateServiceTypeAsync(ServiceType serviceType);
        Task<bool> DeleteServiceCenterAsync(int serviceCenterId);
        Task<bool> DeleteServiceSystemAsync(int serviceSystemId);
        Task<bool> DeleteServiceTypeAsync(int serviceTypeId);
        Task<ServiceCenter> GetServiceCenterAsync(int serviceCenterId);
        Task<List<ServiceCenter>> GetServiceCentersAsync();
        Task<ServiceSystem> GetServiceSystemAsync(int serviceSystemId);
        Task<List<ServiceSystem>> GetServiceSystemsAsync();
        Task<ServiceType> GetServiceTypeAsync(int serviceTypeId);
        Task<List<ServiceType>> GetServiceTypesAsync();
        Task<bool> UpdateServiceCenterAsync(ServiceCenter serviceCenter);
        Task<bool> UpdateServiceSystemAsync(ServiceSystem serviceSystem);
        Task<bool> UpdateServiceTypeAsync(ServiceType serviceType);

        Task<List<ServiceRequestActivity>> GetServiceRequestActivitiesAsync(long serviceIncidentId);
        Task<bool> AddServiceRequestNoteAsync(ServiceRequestNote serviceRequestNote);
        Task<List<ServiceRequestNote>> GetServiceRequestNotesAsync(long serviceIncidentId);
        #endregion

        #region Utility Action Interfaces
        Task<string> GetIncidentCodeNumber();
        #endregion
    }
}