using IntranetPortal.Base.Models.SrmModels;
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
        //private readonly IEmployeesRepository _employeesRepository;

        public RequestService(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

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

    }
}
