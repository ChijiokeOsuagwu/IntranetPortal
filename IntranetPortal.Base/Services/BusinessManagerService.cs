using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class BusinessManagerService : IBusinessManagerService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessContactRepository _businessContactRepository;

        public BusinessManagerService(IBusinessRepository businessRepository, IBusinessContactRepository businessContactRepository)
        {
            _businessRepository = businessRepository;
            _businessContactRepository = businessContactRepository;
        }

        //============================ Business CRUD Action Methods =============================//
        #region Business CRUD Action Methods
        public async Task<bool> CreateBusinessAsync(Business business)
        {
            if (business == null) { throw new ArgumentNullException(nameof(business), "Required parameter [Business] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessRepository.AddAsync(business);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteBusinessAsync(string businessId)
        {
            if (string.IsNullOrWhiteSpace(businessId)) { throw new ArgumentNullException(nameof(businessId), "Required parameter [BusinessID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessRepository.DeleteAsync(businessId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateBusinessAsync(Business business)
        {
            if (business == null) { throw new ArgumentNullException(nameof(business), "Required parameter [Business] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessRepository.EditAsync(business);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //============================ Business Contacts Action Methods =========================// 
        #region Business Contacts Action Methods
        public async Task<bool> CreateBusinessContactAsync(BusinessContact businessContact)
        {
            if (businessContact == null) { throw new ArgumentNullException(nameof(businessContact), "Required parameter [Business Contact] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessContactRepository.AddAsync(businessContact);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateBusinessContactAsync(BusinessContact businessContact)
        {
            if (businessContact == null) { throw new ArgumentNullException(nameof(businessContact), "Required parameter [Business Contact] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessContactRepository.EditAsync(businessContact);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteBusinessContactAsync(int businessContactId)
        {
            if (businessContactId < 1) { throw new ArgumentNullException(nameof(businessContactId), "Required parameter [BusinessContactID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessContactRepository.DeleteAsync(businessContactId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteBusinessContactsAsync(string businessId)
        {
            if (string.IsNullOrWhiteSpace(businessId)) { throw new ArgumentNullException(nameof(businessId), "Required parameter [BusinessID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessContactRepository.DeleteByBusinessIdAsync(businessId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<IList<BusinessContact>> GetBusinessContactsAsync()
        {
            IList<BusinessContact> contacts = new List<BusinessContact>();
            try
            {
                var entities = await _businessContactRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { contacts = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return contacts;
        }
        public async Task<IList<BusinessContact>> GetBusinessContactsByBusinessIdAsync(string businessId)
        {
            IList<BusinessContact> contacts = new List<BusinessContact>();
            try
            {
                var entities = await _businessContactRepository.GetByBusinessIdAsync(businessId);
                if (entities != null && entities.Count > 0) { contacts = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return contacts;
        }
        public async Task<BusinessContact> GetBusinessContactByIdAsync(int businessContactId)
        {
            BusinessContact contact = new BusinessContact();
            try
            {
                var entity = await _businessContactRepository.GetByIdAsync(businessContactId);
                if (entity != null) { contact = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return contact;
        }
        #endregion

        //============================ Customers Action Methods =============================//
        #region Customers Action Methods
        public async Task<IList<Business>> GetCustomersAsync()
        {
            IList<Business> customers = new List<Business>();
            try
            {
                var entities = await _businessRepository.GetAllCustomersAsync();
                if (entities != null && entities.Count > 0) { customers = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customers;
        }

        public async Task<Business> GetCustomerByIdAsync(string customerId)
        {
            Business customer = new Business();
            try
            {
                var entity = await _businessRepository.GetByIdAsync(customerId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { customer = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customer;
        }

        public async Task<Business> GetCustomerByNameAsync(string customerName)
        {
            Business customer = new Business();
            try
            {
                var entity = await _businessRepository.GetCustomerByNameAsync(customerName);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { customer = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customer;
        }

        public async Task<IList<Business>> SearchCustomersByNameAsync(string customerName)
        {
            IList<Business> customers = new List<Business>();
            try
            {
                var entities = await _businessRepository.SearchCustomersByNameAsync(customerName);
                if (entities != null && entities.Count > 0) { customers = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customers;
        }

        #endregion

        //============================ Suppliers Action Methods =============================//
        #region Suppliers Action Methods
        public async Task<IList<Business>> GetSuppliersAsync()
        {
            IList<Business> suppliers = new List<Business>();
            try
            {
                var entities = await _businessRepository.GetAllSuppliersAsync();
                if (entities != null && entities.Count > 0) { suppliers = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return suppliers;
        }

        public async Task<Business> GetSupplierByIdAsync(string supplierId)
        {
            Business supplier = new Business();
            try
            {
                var entity = await _businessRepository.GetByIdAsync(supplierId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { supplier = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return supplier;
        }

        public async Task<Business> GetSupplierByNameAsync(string supplierName)
        {
            Business supplier = new Business();
            try
            {
                var entity = await _businessRepository.GetSupplierByNameAsync(supplierName);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { supplier = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return supplier;
        }

        public async Task<IList<Business>> SearchSuppliersByNameAsync(string supplierName)
        {
            IList<Business> suppliers = new List<Business>();
            try
            {
                var entities = await _businessRepository.SearchSuppliersByNameAsync(supplierName);
                if (entities != null && entities.Count > 0) { suppliers = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return suppliers;
        }

        #endregion

    }
}
