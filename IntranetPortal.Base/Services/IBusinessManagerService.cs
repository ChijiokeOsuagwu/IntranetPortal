using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBusinessManagerService
    {
        #region Business CRUD Action Methods
        Task<bool> CreateBusinessAsync(Business business);
        Task<bool> DeleteBusinessAsync(string businessId);
        Task<bool> UpdateBusinessAsync(Business business);
        Task<string> GetNewCodeNumber();
        #endregion

        #region Business Contacts Action Methods
        Task<bool> CreateBusinessContactAsync(BusinessContact businessContact);
        Task<bool> UpdateBusinessContactAsync(BusinessContact businessContact);
        Task<bool> DeleteBusinessContactAsync(long businessContactId);
        Task<bool> DeleteBusinessContactsAsync(string businessId);
        Task<List<BusinessContact>> GetBusinessContactsAsync();
        Task<List<BusinessContact>> GetBusinessContactsByBusinessIdAsync(string businessId);
        Task<BusinessContact> GetBusinessContactByIdAsync(long businessContactId);
        #endregion

        #region Customers Action Methods
        Task<List<Business>> GetCustomersAsync();
        Task<Business> GetCustomerByIdAsync(string customerId);
        Task<Business> GetCustomerByNameAsync(string customerName);
        Task<List<Business>> SearchCustomersByNameAsync(string customerName);
        #endregion

        #region Suppliers Action Methods
        Task<List<Business>> GetSuppliersAsync();
        Task<Business> GetSupplierByIdAsync(string supplierId);
        Task<Business> GetSupplierByNameAsync(string supplierName);
        Task<List<Business>> SearchSuppliersByNameAsync(string supplierName);
        #endregion
    }
}
