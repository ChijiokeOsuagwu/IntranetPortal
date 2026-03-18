using IntranetPortal.Base.Models.SrmModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SrmRepositories
{
    public interface IServiceRequestRepository
    {
        IConfiguration _config { get; }

        #region Service Incident Action Methods

        #region Write Action Methods
        Task<long> AddServiceIncidentAsync(ServiceIncident incident);
        Task<bool> UpdateServiceIncidentAsync(ServiceIncident incident);
        Task<bool> UpdateServiceIncidentStatusAsync(long serviceIncidentId, string newIncidentStatus);
        Task<bool> DeleteServiceIncidentAsync(long serviceIncidentId);
        #endregion

        #region Read Action Methods
        Task<List<ServiceIncident>> GetServiceIncidentsByOwnerIdAsync(string ownerId, DateTime? startDate, DateTime? endDate);
        Task<List<ServiceIncident>> GetServiceIncidentsByTeamMemberIdAsync(string teamMemberId, DateTime? startDate, DateTime? endDate);

        Task<List<ServiceIncident>> GetServiceIncidentByIdAsync(long serviceIncidentId);
        #endregion

        #endregion

        #region Incident Resolution Action Interfaces
        #region Read Interfaces
        Task<List<IncidentResolution>> GetIncidentResolutionsByIncidentIdAsync(long incidentId);
        Task<List<IncidentResolution>> GetIncidentResolutionsByIdAsync(long incidentResolutionId);
        #endregion

        #region Write Interfaces
        Task<long> AddIncidentResolutionAsync(IncidentResolution resolution);
        Task<bool> UpdateIncidentResolutionAsync(IncidentResolution resolution);
        Task<bool> UpdateIncidentResolutionConfirmationAsync(long incidentResolutionId, bool resolutionIsConfirmed, string resolutionConfirmedBy);
        Task<bool> DeleteIncidentResolutionAsync(long incidentResolutionId);
        #endregion
        
        #endregion

        #region Service Settings

        #region Service Systems Action Methods
        Task<int> AddServiceSystemAsync(ServiceSystem system);
        Task<bool> DeleteServiceSystemAsync(int systemId);
        Task<bool> UpdateServiceSystemAsync(ServiceSystem system);


        Task<List<ServiceSystem>> GetServiceSystemByIdAsync(int systemId);
        Task<List<ServiceSystem>> GetServiceSystemsAsync();
        Task<List<ServiceSystem>> GetServiceSystemsByNameAsync(string name);

        #endregion

        #region Service Centers Action Methods
        Task<int> AddServiceCenterAsync(ServiceCenter center);
        Task<bool> UpdateServiceCenterAsync(ServiceCenter center);
        Task<bool> DeleteServiceCenterAsync(int serviceCenterId);

        Task<List<ServiceCenter>> GetServiceCenterByIdAsync(int serviceCenterId);
        Task<List<ServiceCenter>> GetServiceCentersAsync();
        Task<List<ServiceCenter>> GetServiceCentersByNameAsync(string serviceCenterName);

        #endregion

        #region Service Types Action Methods
        Task<long> AddServiceTypeAsync(ServiceType serviceType);
        Task<bool> UpdateServiceTypeAsync(ServiceType serviceType);
        Task<bool> DeleteServiceTypeAsync(int serviceTypeId);

        Task<ServiceType> GetServiceTypeByIdAsync(int serviceTypeId);
        Task<List<ServiceType>> GetServiceTypesAsync();
        Task<ServiceType> GetServiceTypesByNameAsync(string serviceTypeName);

        #endregion

        #region Service Request Note Action Methods
        Task<bool> AddNoteAsync(ServiceRequestNote n);
        Task<bool> UpdateServiceRequestNoteToIsCancelledAsync(long serviceRequestNoteId, bool isCancelled, string cancelledBy);
        Task<bool> DeleteServiceRequestNoteAsync(long serviceRequestNoteId);

        Task<List<ServiceRequestNote>> GetServiceRequestNotesByIncidentIdAsync(long serviceIncidentId);
        #endregion

        #region Service Request Activity Log Action Methods
        Task<bool> AddServiceRequestActivityAsync(ServiceRequestActivity log);
        Task<bool> DeleteServiceRequestActivityAsync(long activityLogId);
        Task<List<ServiceRequestActivity>> GetServiceRequestActivitysByServiceIncidentIdAsync(long serviceIncidentId);
        #endregion

        #region Utility Action Methods
        Task<List<string>> GetIncidentCodeNumbersByCreatedDateAsync(DateTime createdDate);
        #endregion

        #endregion
    }
}