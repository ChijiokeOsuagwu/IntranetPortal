using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BusinessManagerRepositories
{
    public interface IBusinessRepository
    {
        Task<Business> GetCustomerByNameAsync(string businessName);
        Task<List<Business>> SearchCustomersByNameAsync(string businessName);
        Task<List<Business>> GetAllCustomersAsync();
        Task<Business> GetSupplierByNameAsync(string businessName);
        Task<List<Business>> SearchSuppliersByNameAsync(string businessName);
        Task<List<Business>> GetAllSuppliersAsync();
        Task<bool> AddAsync(Business business);
        Task<bool> EditAsync(Business business);
        Task<bool> DeleteAsync(string businessId);

        //===== General Action Interfaces =======//
        Task<List<string>> GetCodeNumbersByCreatedDateAsync(DateTime createdDate);
        Task<Business> GetByIdAsync(string businessId);

    }
}
