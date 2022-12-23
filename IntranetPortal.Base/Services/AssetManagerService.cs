using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class AssetManagerService : IAssetManagerService
    {
        private readonly IAssetCategoryRepository _assetCategoryRepository;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IAssetReservationRepository _assetReservationRepository;
        private readonly IAssetUsageRepository _assetUsageRepository;
        private readonly IAssetIncidentRepository _assetIncidentRepository;
        private readonly IAssetMaintenanceRepository _assetMaintenanceRepository;
        private readonly IAssetMovementRepository _assetMovementRepository;

        public AssetManagerService(IAssetCategoryRepository assetCategoryRepository, IAssetTypeRepository assetTypeRepository,
                                        IAssetRepository assetRepository, IAssetReservationRepository assetReservationRepository,
                                        IAssetUsageRepository assetUsageRepository, IAssetIncidentRepository assetIncidentRepository,
                                        IAssetMaintenanceRepository assetMaintenanceRepository, IAssetMovementRepository assetMovementRepository)
        {
            _assetCategoryRepository = assetCategoryRepository;
            _assetTypeRepository = assetTypeRepository;
            _assetRepository = assetRepository;
            _assetReservationRepository = assetReservationRepository;
            _assetUsageRepository = assetUsageRepository;
            _assetIncidentRepository = assetIncidentRepository;
            _assetMaintenanceRepository = assetMaintenanceRepository;
            _assetMovementRepository = assetMovementRepository;
        }

        //=================== Asset Category Action Methods ======================//
        #region Asset Category Action Methods

        public async Task<bool> CreateAssetCategoryAsync(AssetCategory assetCategory)
        {
            if (assetCategory == null) { throw new ArgumentNullException(nameof(assetCategory), "Required parameter [AssetCategory] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetCategoryRepository.AddAsync(assetCategory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetCategoryAsync(int assetCategoryId)
        {
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "Required parameter [AssetCategoryID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetCategoryRepository.DeleteAsync(assetCategoryId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetCategoryAsync(AssetCategory assetCategory)
        {
            if (assetCategory == null) { throw new ArgumentNullException(nameof(assetCategory), "Required parameter [AssetCategory] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetCategoryRepository.EditAsync(assetCategory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync()
        {
            List<AssetCategory> assetCategories = new List<AssetCategory>();
            try
            {
                var entities = await _assetCategoryRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetCategories = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetCategories;
        }

        public async Task<AssetCategory> GetAssetCategoryByIdAsync(int assetCategoryId)
        {
            AssetCategory assetCategory = new AssetCategory();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [assetCateogryId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetCategoryRepository.GetByIdAsync(assetCategoryId);
                if (entity != null) { assetCategory = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetCategory;
        }

        public async Task<IList<AssetCategory>> SearchAssetCategoriesByNameAsync(string assetCategoryName)
        {
            List<AssetCategory> assetCategories = new List<AssetCategory>();
            try
            {
                var entities = await _assetCategoryRepository.GetByNameAsync(assetCategoryName);
                if (entities != null && entities.Count > 0) { assetCategories = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetCategories;
        }

        #endregion

        //=================== Asset Type Action Methods ==========================//
        #region Asset Type Action Methods

        public async Task<bool> CreateAssetTypeAsync(AssetType assetType)
        {
            if (assetType == null) { throw new ArgumentNullException(nameof(assetType), "Required parameter [AssetType] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetTypeRepository.AddAsync(assetType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetTypeAsync(int assetTypeId)
        {
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "Required parameter [AssetTypeID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetTypeRepository.DeleteAsync(assetTypeId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetTypeAsync(AssetType assetType)
        {
            if (assetType == null) { throw new ArgumentNullException(nameof(assetType), "Required parameter [AssetType] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetTypeRepository.EditAsync(assetType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetType>> GetAssetTypesAsync()
        {
            List<AssetType> assetTypes = new List<AssetType>();
            try
            {
                var entities = await _assetTypeRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetTypes;
        }

        public async Task<AssetType> GetAssetTypeByIdAsync(int assetTypeId)
        {
            AssetType assetType = new AssetType();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [assetTypeId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetTypeRepository.GetByIdAsync(assetTypeId);
                if (entity != null) { assetType = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetType;
        }

        public async Task<IList<AssetType>> SearchAssetTypesByNameAsync(string assetTypeName)
        {
            List<AssetType> assetTypes = new List<AssetType>();
            try
            {
                var entities = await _assetTypeRepository.GetByNameAsync(assetTypeName);
                if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetTypes;
        }

        public async Task<IList<AssetType>> GetAssetTypesByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetType> assetTypes = new List<AssetType>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetTypeRepository.GetByCategoryIdAsync(assetCategoryId);
                if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetTypes;
        }

        #endregion

        //=================== Asset Action Methods ===============================//
        #region Asset Action Methods

        public async Task<bool> CreateAssetAsync(Asset asset)
        {
            if (asset == null) { throw new ArgumentNullException(nameof(asset), "Required parameter [Asset] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetRepository.AddAsync(asset);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetAsync(string assetId, string deletedBy)
        {
            if (string.IsNullOrWhiteSpace(assetId)) { throw new ArgumentNullException(nameof(assetId), "Required parameter [AssetID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetRepository.DeleteAsync(assetId, deletedBy);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetAsync(Asset asset)
        {
            if (asset == null) { throw new ArgumentNullException(nameof(asset), "Required parameter [Asset] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetRepository.EditAsync(asset);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<Asset>> GetAssetsAsync()
        {
            List<Asset> assets = new List<Asset>();
            try
            {
                var entities = await _assetRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<Asset> GetAssetByIdAsync(string assetId)
        {
            Asset asset = new Asset();
            if (string.IsNullOrWhiteSpace(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [assetId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetRepository.GetByIdAsync(assetId);
                if (entity != null) { asset = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return asset;
        }

        public async Task<Asset> GetAssetByNameAsync(string assetName)
        {
            Asset asset = new Asset();
            if (string.IsNullOrWhiteSpace(assetName)) { throw new ArgumentNullException(nameof(assetName), "The required parameter [assetName] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetRepository.GetByNameAsync(assetName);
                if (entity != null) { asset = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return asset;
        }

        public async Task<IList<Asset>> SearchAssetsByNameAsync(string assetName)
        {
            List<Asset> assets = new List<Asset>();
            try
            {
                var entities = await _assetRepository.SearchByNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByAssetTypeIdAsync(int assetTypeId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByCategoryIdAsync(int assetCategoryId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByCategoryIdAsync(assetCategoryId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        #endregion

        //=================== Asset Reservation Action Methods ===================//
        #region Asset Reservation Action Methods

        public async Task<bool> CreateAssetReservationAsync(AssetReservation assetReservation)
        {
            if (assetReservation == null) { throw new ArgumentNullException(nameof(assetReservation), "Required parameter [AssetReservation] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetReservationRepository.AddAsync(assetReservation);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetReservationAsync(int assetReservationId)
        {
            if (assetReservationId < 1) { throw new ArgumentNullException(nameof(assetReservationId), "Required parameter [AssetReservationID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetReservationRepository.DeleteAsync(assetReservationId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetReservationAsync(AssetReservation assetReservation)
        {
            if (assetReservation == null) { throw new ArgumentNullException(nameof(assetReservation), "Required parameter [AssetReservation] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetReservationRepository.EditAsync(assetReservation);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetReservation>> GetAssetReservationsAsync()
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            try
            {
                var entities = await _assetReservationRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<AssetReservation> GetAssetReservationByIdAsync(int assetReservationId)
        {
            AssetReservation assetReservation = new AssetReservation();
            if (assetReservationId < 1) { throw new ArgumentNullException(nameof(assetReservationId), "The required parameter [AssetReservationId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetReservationRepository.GetByIdAsync(assetReservationId);
                if (entity != null) { assetReservation = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservation;
        }

        public async Task<IList<AssetReservation>> GetAssetReservationsByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetReservationRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetAssetReservationsByAssetIdAsync(string assetId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetReservationRepository.GetByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> SearchAssetReservationsByAssetNameAsync(string assetName)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            try
            {
                var entities = await _assetReservationRepository.GetByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetAssetReservationsByAssetIdAndYearAsync(string assetId, int reservedYear)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if(reservedYear < 1) { throw new ArgumentException(nameof(reservedYear)); }
            try
            {
                var entities = await _assetReservationRepository.GetByAssetIdAndYearAsync(assetId, reservedYear);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetAssetReservationsByAssetIdAndYearAndMonthAsync(string assetId, int reservedYear, int reservedMonth)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if (reservedYear < 1) { throw new ArgumentException(nameof(reservedYear)); }
            if(reservedMonth < 1) { throw new ArgumentException(nameof(reservedMonth)); }
            try
            {
                var entities = await _assetReservationRepository.GetByAssetIdAndYearAndMonthAsync(assetId, reservedYear, reservedMonth);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }


        //==================== Current Asset Reservations Only ====================================================//
        public async Task<IList<AssetReservation>> GetCurrentAssetReservationsAsync()
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            try
            {
                var entities = await _assetReservationRepository.GetAllCurrentAsync();
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }
        public async Task<IList<AssetReservation>> GetCurrentAssetReservationsByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetReservationRepository.GetCurrentByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> GetCurrentAssetReservationsByAssetIdAsync(string assetId)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetReservationRepository.GetCurrentByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        public async Task<IList<AssetReservation>> SearchCurrentAssetReservationsByAssetNameAsync(string assetName)
        {
            List<AssetReservation> assetReservations = new List<AssetReservation>();
            try
            {
                var entities = await _assetReservationRepository.GetCurrentByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetReservations = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetReservations;
        }

        #endregion

        //=================== Asset Usage Action Methods =========================//
        #region Asset Usage Action Methods
        //=================== Current Asset Usage Methods Only ==================//
        public async Task<IList<AssetUsage>> GetCurrentAssetUsagesAsync()
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            try
            {
                var entities = await _assetUsageRepository.GetAllCurrentAsync();
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCurrentAssetUsagesByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetUsageRepository.GetCurrentByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCurrentAssetUsagesByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetUsageRepository.GetCurrentByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> SearchCurrentAssetUsagesByAssetNameAsync(string assetName)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            try
            {
                var entities = await _assetUsageRepository.GetCurrentByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }


        //======================= Asset Usage Action Methods ======================================================//
        public async Task<bool> CheckOutEquipmentAsync(AssetUsage assetUsage)
        {
            if (assetUsage == null) { throw new ArgumentNullException(nameof(assetUsage), "Required parameter [AssetUsage] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetUsageRepository.AddCheckOutAsync(assetUsage);
                if (firstSuccess)
                {
                    string usageStatus = "Unavailable";
                    switch (assetUsage.Purpose)
                    {
                        case "Assignment":
                            usageStatus = "In Use";
                            break;
                        case "Repair":
                            usageStatus = "Unavailable";
                            break;
                        default:
                            usageStatus = "Unavailable";
                            break;
                    }
                    await _assetRepository.UpdateUsageStatusAsync(assetUsage.AssetID, usageStatus, assetUsage.ModifiedBy);
                    await _assetRepository.UpdateCurrentLocationAsync(assetUsage.AssetID, assetUsage.UsageLocation, assetUsage.ModifiedBy);
                    IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> CancelCheckOutEquipmentAsync(int assetUsageId, string assetId, string previousLocation, string previousStatus, string modifiedBy)
        {
            if (assetUsageId < 1) { throw new ArgumentNullException(nameof(assetUsageId), "Required parameter [AssetUsageID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                //var assetUsage = await _assetUsageRepository.GetByIdAsync(assetUsageId);
                IsSuccessful = await _assetUsageRepository.DeleteAsync(assetUsageId);
                if (IsSuccessful)
                {
                    if (!string.IsNullOrWhiteSpace(previousStatus))
                    {
                        await _assetRepository.UpdateUsageStatusAsync(assetId, previousStatus, modifiedBy);
                    }

                    if(!string.IsNullOrWhiteSpace(previousLocation))
                    {
                        await _assetRepository.UpdateCurrentLocationAsync(assetId, previousLocation, modifiedBy);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetUsageAsync(int assetUsageId)
        {
            if (assetUsageId < 1) { throw new ArgumentNullException(nameof(assetUsageId), "Required parameter [AssetUsageID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetReservationRepository.DeleteAsync(assetUsageId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetUsageAsync(AssetUsage assetUsage)
        {
            if (assetUsage == null) { throw new ArgumentNullException(nameof(assetUsage), "Required parameter [AssetUsage] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetUsageRepository.EditAsync(assetUsage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetUsage>> GetAssetUsagesAsync()
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            try
            {
                var entities = await _assetUsageRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<AssetUsage> GetAssetUsageByIdAsync(int assetUsageId)
        {
            AssetUsage assetUsage = new AssetUsage();
            if (assetUsageId < 1) { throw new ArgumentNullException(nameof(assetUsageId), "The required parameter [AssetUsageId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetUsageRepository.GetByIdAsync(assetUsageId);
                if (entity != null) { assetUsage = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsage;
        }

        public async Task<IList<AssetUsage>> GetAssetUsagesByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetUsageRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetAssetUsagesByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetUsageRepository.GetByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetAssetUsagesByAssetIdAndDateAsync(string assetId, int? usageYear = null, int? usageMonth = null)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                if(usageYear != null && usageYear > 0)
                {
                    if(usageMonth != null && usageMonth > 0)
                    {
                        var entities = await _assetUsageRepository.GetByAssetIdAndYearAndMonthAsync(assetId, usageYear.Value, usageMonth.Value);
                        if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
                    }
                    else
                    {
                        var entities = await _assetUsageRepository.GetByAssetIdAndYearAsync(assetId, usageYear.Value);
                        if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
                    }
                }
                else
                {
                    if(usageMonth != null & usageMonth > 0 && usageMonth < 13)
                    {
                        int usageYearInt = DateTime.Now.Year;
                        var entities = await _assetUsageRepository.GetByAssetIdAndYearAndMonthAsync(assetId, usageYearInt, usageMonth.Value);
                        if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
                    }
                    else
                    {
                        var entities = await _assetUsageRepository.GetByAssetIdAsync(assetId);
                        if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetAssetUsagesCheckedOutByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetUsageRepository.GetCheckedOutByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> SearchAssetUsagesByAssetNameAsync(string assetName)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            try
            {
                var entities = await _assetUsageRepository.GetByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetUsages = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetUsages;
        }

        #endregion

        //=================== Asset Incident Action Methods =========================//
        #region Asset Incident Action Methods
        //========== Current Asset Incident Methods Only ================//
        public async Task<IList<AssetIncident>> GetCurrentAssetIncidentsAsync()
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            try
            {
                var entities = await _assetIncidentRepository.GetAllCurrentAsync();
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetCurrentAssetIncidentsByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetIncidentRepository.GetCurrentByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetCurrentAssetIncidentsByAssetIdAsync(string assetId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetIncidentRepository.GetCurrentByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> SearchCurrentAssetIncidentsByAssetNameAsync(string assetName)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            try
            {
                var entities = await _assetIncidentRepository.GetCurrentByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        //========== Asset Incident Action Methods ======================//
        public async Task<bool> AddAssetIncidentAsync(AssetIncident assetIncident)
        {
            if (assetIncident == null) { throw new ArgumentNullException(nameof(assetIncident), "Required parameter [AssetIncident] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetIncidentRepository.AddAssetIncidentAsync(assetIncident);
                if (firstSuccess)
                {
                    await _assetRepository.UpdateAssetConditionAsync(assetIncident.AssetID, assetIncident.AssetCondition, assetIncident.ModifiedBy);
                    IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetIncidentAsync(int assetIncidentId)
        {
            if (assetIncidentId < 1) { throw new ArgumentNullException(nameof(assetIncidentId), "Required parameter [AssetIncidentID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetIncidentRepository.DeleteAsync(assetIncidentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetIncidentAsync(AssetIncident assetIncident)
        {
            if (assetIncident == null) { throw new ArgumentNullException(nameof(assetIncident), "Required parameter [AssetUsage] is missing."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetIncidentRepository.EditAsync(assetIncident);
                if (firstSuccess)
                {
                    return await _assetRepository.UpdateAssetConditionAsync(assetIncident.AssetID, assetIncident.AssetCondition, assetIncident.ModifiedBy);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetIncident>> GetAssetIncidentsAsync()
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            try
            {
                var entities = await _assetIncidentRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<AssetIncident> GetAssetIncidentByIdAsync(int assetIncidentId)
        {
            AssetIncident assetIncident = new AssetIncident();
            if (assetIncidentId < 1) { throw new ArgumentNullException(nameof(assetIncidentId), "The required parameter [AssetIncidentId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetIncidentRepository.GetByIdAsync(assetIncidentId);
                if (entity != null) { assetIncident = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncident;
        }

        public async Task<IList<AssetIncident>> GetAssetIncidentsByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetIncidentRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetAssetIncidentsByAssetIdAsync(string assetId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetIncidentRepository.GetByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> SearchAssetIncidentsByAssetNameAsync(string assetName)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            try
            {
                var entities = await _assetIncidentRepository.GetByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetAssetIncidentsByAssetIdAndYearAsync(string assetId, int incidentYear)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if(incidentYear < 1) { throw new ArgumentException(nameof(incidentYear)); }
            
            try
            {
                var entities = await _assetIncidentRepository.GetByAssetIdAndYearAsync(assetId, incidentYear);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetAssetIncidentsByAssetIdAndYearAndMonthAsync(string assetId, int incidentYear, int incidentMonth)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if (incidentYear < 1) { throw new ArgumentException(nameof(incidentYear)); }
            if (incidentMonth < 1) { throw new ArgumentException(nameof(incidentMonth)); }

            try
            {
                var entities = await _assetIncidentRepository.GetByAssetIdAndYearAndMonthAsync(assetId, incidentYear, incidentMonth);
                if (entities != null && entities.Count > 0) { assetIncidents = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetIncidents;
        }

        #endregion

        //=================== Asset Maintenance Action Methods ======================//
        #region Asset Maintenance Action Methods
        //========== Current Asset Maintenance Methods Only =========================//
        public async Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesAsync()
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            try
            {
                var entities = await _assetMaintenanceRepository.GetAllCurrentAsync();
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetCurrentByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesByAssetIdAsync(string assetId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetCurrentByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> SearchCurrentAssetMaintenancesByAssetNameAsync(string assetName)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            try
            {
                var entities = await _assetMaintenanceRepository.GetCurrentByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        //========== Asset Maintenance Action Methods ======================//
        public async Task<bool> AddAssetMaintenanceAsync(AssetMaintenance assetMaintenance)
        {
            if (assetMaintenance == null) { throw new ArgumentNullException(nameof(assetMaintenance), "Required parameter [AssetMaintenance] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetMaintenanceRepository.AddAssetMaintenanceAsync(assetMaintenance);
                if (firstSuccess)
                {
                    await _assetRepository.UpdateAssetConditionAsync(assetMaintenance.AssetID, assetMaintenance.FinalCondition, assetMaintenance.ModifiedBy);
                    IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetMaintenanceAsync(int assetMaintenanceId)
        {
            if (assetMaintenanceId < 1) { throw new ArgumentNullException(nameof(assetMaintenanceId), "Required parameter [AssetMaintenanceID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetMaintenanceRepository.DeleteAsync(assetMaintenanceId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetMaintenanceAsync(AssetMaintenance assetMaintenance)
        {
            if (assetMaintenance == null) { throw new ArgumentNullException(nameof(assetMaintenance), "Required parameter [AssetMaintenance] is missing."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetMaintenanceRepository.EditAsync(assetMaintenance);
                if (firstSuccess)
                {
                    return await _assetRepository.UpdateAssetConditionAsync(assetMaintenance.AssetID, assetMaintenance.FinalCondition, assetMaintenance.ModifiedBy);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetMaintenance>> GetAssetMaintenancesAsync()
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            try
            {
                var entities = await _assetMaintenanceRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<AssetMaintenance> GetAssetMaintenanceByIdAsync(int assetMaintenanceId)
        {
            AssetMaintenance assetMaintenance = new AssetMaintenance();
            if (assetMaintenanceId < 1) { throw new ArgumentNullException(nameof(assetMaintenanceId), "The required parameter [AssetMaintenanceId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetMaintenanceRepository.GetByIdAsync(assetMaintenanceId);
                if (entity != null) { assetMaintenance = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenance;
        }

        public async Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetIdAsync(string assetId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetIdAndYearAsync(string assetId, int maintenanceYear)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if(maintenanceYear < 1) { throw new ArgumentException(nameof(maintenanceYear)); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetByAssetIdAndYearAsync(assetId, maintenanceYear);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetIdAndYearAndMonthAsync(string assetId, int maintenanceYear, int maintenanceMonth)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if (maintenanceYear < 1) { throw new ArgumentException(nameof(maintenanceYear)); }
            if(maintenanceMonth < 1) { throw new ArgumentException(nameof(maintenanceMonth)); }
            try
            {
                var entities = await _assetMaintenanceRepository.GetByAssetIdAndYearAndMonthAsync(assetId, maintenanceYear, maintenanceMonth);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> SearchAssetMaintenancesByAssetNameAsync(string assetName)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            try
            {
                var entities = await _assetMaintenanceRepository.GetByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetMaintenanceList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMaintenanceList;
        }

        #endregion

        //=================== Asset Movement Action Methods =========================//
        #region Asset Movement Action Methods

        public async Task<IList<AssetMovement>> SearchAssetMovementsByAssetNameAsync(string assetName)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            try
            {
                var entities = await _assetMovementRepository.GetByAssetNameAsync(assetName);
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        public async Task<bool> AddAssetMovementAsync(AssetMovement assetMovement)
        {
            if (assetMovement == null) { throw new ArgumentNullException(nameof(assetMovement), "Required parameter [AssetMovement] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetMovementRepository.AddAssetMovementAsync(assetMovement);
                if (firstSuccess)
                {
                    await _assetRepository.UpdateBaseLocationAsync(assetMovement.AssetID, assetMovement.MovedToLocationID.Value, assetMovement.MovedToLocationName, assetMovement.ModifiedBy);
                    IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetMovementAsync(int assetMovementId)
        {
            if (assetMovementId < 1) { throw new ArgumentNullException(nameof(assetMovementId), "Required parameter [AssetMovementID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetMovementRepository.DeleteAsync(assetMovementId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetMovementAsync(AssetMovement assetMovement)
        {
            if (assetMovement == null) { throw new ArgumentNullException(nameof(assetMovement), "Required parameter [AssetMovement] is missing."); }
            bool IsSuccessful = false;
            try
            {
                bool firstSuccess = await _assetMovementRepository.EditAsync(assetMovement);
                if (firstSuccess)
                {
                    await _assetRepository.UpdateBaseLocationAsync(assetMovement.AssetID, assetMovement.MovedToLocationID.Value, assetMovement.MovedToLocationName, assetMovement.ModifiedBy);
                    IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetMovement>> GetAssetMovementAsync()
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            try
            {
                var entities = await _assetMovementRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        public async Task<AssetMovement> GetAssetMovementByIdAsync(int assetMovementId)
        {
            AssetMovement assetMovement = new AssetMovement();
            if (assetMovementId < 1) { throw new ArgumentNullException(nameof(assetMovementId), "The required parameter [AssetMovementId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetMovementRepository.GetByIdAsync(assetMovementId);
                if (entity != null) { assetMovement = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovement;
        }

        public async Task<IList<AssetMovement>> GetAssetMovementsByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMovementRepository.GetByAssetTypeIdAsync(assetTypeId);
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        public async Task<IList<AssetMovement>> GetAssetMovementsByAssetIdAsync(string assetId)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetMovementRepository.GetByAssetIdAsync(assetId);
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        public async Task<IList<AssetMovement>> GetAssetMovementsByAssetIdAndYearAsync(string assetId, int movementYear)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if(movementYear < 1) { throw new ArgumentException(nameof(movementYear)); }
            try
            {
                var entities = await _assetMovementRepository.GetByAssetIdAndYearAsync(assetId, movementYear);
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        public async Task<IList<AssetMovement>> GetAssetMovementsByAssetIdAndYearAndMonthAsync(string assetId, int movementYear, int movementMonth)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId), "The required parameter [AssetID] is missing. The request cannot be processed."); }
            if (movementYear < 1) { throw new ArgumentException(nameof(movementYear)); }
            if (movementMonth < 1) { throw new ArgumentException(nameof(movementMonth)); }

            try
            {
                var entities = await _assetMovementRepository.GetByAssetIdAndYearAndMonthAsync(assetId, movementYear, movementMonth);
                if (entities != null && entities.Count > 0) { assetMovementList = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetMovementList;
        }

        #endregion
    }
}
