using IntranetPortal.Base.Models.SrmModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IRequestService
    {
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
    }
}