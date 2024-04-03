using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using IntranetPortal.Data.Repositories.AssetManagerRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAssetBinLocationRepository _assetBinLocationRepository;
        private readonly IAssetClassRepository _assetClassRepository;
        private readonly IAssetGroupRepository _assetGroupRepository;
        private readonly IAssetDivisionRepository _assetDivisionRepository;

        public AssetManagerService(IAssetCategoryRepository assetCategoryRepository, IAssetTypeRepository assetTypeRepository,
                                        IAssetRepository assetRepository, IAssetReservationRepository assetReservationRepository,
                                        IAssetUsageRepository assetUsageRepository, IAssetIncidentRepository assetIncidentRepository,
                                        IAssetMaintenanceRepository assetMaintenanceRepository, IAssetMovementRepository assetMovementRepository,
                                        IAssetBinLocationRepository assetBinLocationRepository, IAssetClassRepository assetClassRepository,
                                        IAssetDivisionRepository assetDivisionRepository, IAssetGroupRepository assetGroupRepository)
        {
            _assetCategoryRepository = assetCategoryRepository;
            _assetTypeRepository = assetTypeRepository;
            _assetRepository = assetRepository;
            _assetReservationRepository = assetReservationRepository;
            _assetUsageRepository = assetUsageRepository;
            _assetIncidentRepository = assetIncidentRepository;
            _assetMaintenanceRepository = assetMaintenanceRepository;
            _assetMovementRepository = assetMovementRepository;
            _assetBinLocationRepository = assetBinLocationRepository;
            _assetClassRepository = assetClassRepository;
            _assetGroupRepository = assetGroupRepository;
            _assetDivisionRepository = assetDivisionRepository;
        }


        //============== Asset Division Action Methods ====================//
        #region Asset Division Action Methods

        public async Task<bool> CreateAssetDivisionAsync(AssetDivision assetDivision)
        {
            if (assetDivision == null) { throw new ArgumentNullException(nameof(assetDivision), "Required parameter [AssetDivision] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetDivisionRepository.AddAsync(assetDivision);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetDivisionAsync(int assetDivisionId)
        {
            if (assetDivisionId < 1) { throw new ArgumentNullException(nameof(assetDivisionId), "Required parameter [AssetDivisionID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetDivisionRepository.DeleteAsync(assetDivisionId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetDivisionAsync(AssetDivision assetDivision)
        {
            if (assetDivision == null) { throw new ArgumentNullException(nameof(assetDivision), "Required parameter [AssetCategory] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetDivisionRepository.EditAsync(assetDivision);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetDivision>> GetAssetDivisionsAsync()
        {
            List<AssetDivision> assetDivisions = new List<AssetDivision>();
            try
            {
                var entities = await _assetDivisionRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetDivisions = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetDivisions;
        }

        public async Task<IList<AssetDivision>> GetAssetDivisionsAsync(string userId)
        {
            List<AssetDivision> assetDivisions = new List<AssetDivision>();
            var entities = await _assetDivisionRepository.GetAllAsync(userId);
            if (entities != null && entities.Count > 0) { assetDivisions = entities.ToList(); }
            return assetDivisions;
        }


        public async Task<AssetDivision> GetAssetDivisionByIdAsync(int assetDivisionId)
        {
            AssetDivision assetDivision = new AssetDivision();
            if (assetDivisionId < 1) { throw new ArgumentNullException(nameof(assetDivisionId), "The required parameter [assetCateogryId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetDivisionRepository.GetByIdAsync(assetDivisionId);
                if (entity != null) { assetDivision = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetDivision;
        }

        public async Task<IList<AssetDivision>> SearchAssetDivisionsByNameAsync(string assetDivisionName)
        {
            List<AssetDivision> assetDivisions = new List<AssetDivision>();
            try
            {
                var entities = await _assetDivisionRepository.GetByNameAsync(assetDivisionName);
                if (entities != null && entities.Count > 0) { assetDivisions = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetDivisions;
        }

        #endregion

        //============== Asset Category Action Methods ====================//
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

        //============== Asset Class Action Methods ======================//
        #region Asset Class Action Methods

        public async Task<bool> CreateAssetClassAsync(AssetClass assetClass)
        {
            if (assetClass == null) { throw new ArgumentNullException(nameof(assetClass), "Required parameter [AssetClass] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetClassRepository.AddAsync(assetClass);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetClassAsync(int assetClassId)
        {
            if (assetClassId < 1) { throw new ArgumentNullException(nameof(assetClassId), "Required parameter [AssetClassID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetClassRepository.DeleteAsync(assetClassId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetClassAsync(AssetClass assetClass)
        {
            if (assetClass == null) { throw new ArgumentNullException(nameof(assetClass), "Required parameter [AssetClass] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetClassRepository.EditAsync(assetClass);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<AssetClass>> GetAssetClassesAsync()
        {
            List<AssetClass> assetClasses = new List<AssetClass>();
            try
            {
                var entities = await _assetClassRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { assetClasses = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetClasses;
        }

        public async Task<AssetClass> GetAssetClassByIdAsync(int assetClassId)
        {
            AssetClass assetClass = new AssetClass();
            if (assetClassId < 1) { throw new ArgumentNullException(nameof(assetClassId), "The required parameter [assetClassId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetClassRepository.GetByIdAsync(assetClassId);
                if (entity != null) { assetClass = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetClass;
        }

        public async Task<IList<AssetClass>> GetAssetClassesByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetClass> assetClasses = new List<AssetClass>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }
            var entities = await _assetClassRepository.GetByCategoryIdAsync(assetCategoryId);
            if (entities != null && entities.Count > 0) { assetClasses = entities.ToList(); }
            return assetClasses;
        }

        public async Task<IList<AssetClass>> SearchAssetClassesByNameAsync(string assetClassName)
        {
            List<AssetClass> assetClasses = new List<AssetClass>();
            var entities = await _assetClassRepository.GetByNameAsync(assetClassName);
            if (entities != null && entities.Count > 0) { assetClasses = entities.ToList(); }
            return assetClasses;
        }

        #endregion

        //============== Asset Type Action Methods =======================//
        #region Asset Type Action Methods

        public async Task<bool> CreateAssetTypeAsync(AssetType assetType)
        {
            if (assetType == null) { throw new ArgumentNullException(nameof(assetType), "Required parameter [AssetType] is missing. The request cannot be processed."); }
            return await _assetTypeRepository.AddAsync(assetType);
        }

        public async Task<bool> DeleteAssetTypeAsync(int assetTypeId)
        {
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "Required parameter [AssetTypeID] is missing."); }
                return await _assetTypeRepository.DeleteAsync(assetTypeId);
        }

        public async Task<bool> UpdateAssetTypeAsync(AssetType assetType)
        {
            if (assetType == null) { throw new ArgumentNullException(nameof(assetType), "Required parameter [AssetType] is missing."); }
                return await _assetTypeRepository.EditAsync(assetType);
        }

        public async Task<IList<AssetType>> GetAssetTypesAsync()
        {
            List<AssetType> assetTypes = new List<AssetType>();
            var entities = await _assetTypeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            return assetTypes;
        }

        public async Task<AssetType> GetAssetTypeByIdAsync(int assetTypeId)
        {
            AssetType assetType = new AssetType();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [assetTypeId] is missing. The request cannot be processed."); }

                var entity = await _assetTypeRepository.GetByIdAsync(assetTypeId);
                if (entity != null) { assetType = entity; }
            return assetType;
        }

        public async Task<IList<AssetType>> SearchAssetTypesByNameAsync(string assetTypeName)
        {
            List<AssetType> assetTypes = new List<AssetType>();

                    var entities = await _assetTypeRepository.GetByNameAsync(assetTypeName);
                    if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
 
            return assetTypes;
        }

        public async Task<IList<AssetType>> GetAssetTypesByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetType> assetTypes = new List<AssetType>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }

                    var entities = await _assetTypeRepository.GetByCategoryIdAsync(assetCategoryId);
                    if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }

            return assetTypes;
        }

        public async Task<IList<AssetType>> GetAssetTypesByClassIdAsync(int assetClassId)
        {
            List<AssetType> assetTypes = new List<AssetType>();
            if (assetClassId < 1) { throw new ArgumentNullException(nameof(assetClassId), "The required parameter [AssetClassID] is missing. The request cannot be processed."); }

                    var entities = await _assetTypeRepository.GetByClassIdAsync(assetClassId);
                    if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            return assetTypes;
        }

        public async Task<IList<AssetType>> GetAssetTypesByGroupIdAsync(int assetGroupId)
        {
            List<AssetType> assetTypes = new List<AssetType>();
            if (assetGroupId < 1) { throw new ArgumentNullException(nameof(assetGroupId), "The required parameter [AssetGroupID] is missing. The request cannot be processed."); }

            var entities = await _assetTypeRepository.GetByGroupIdAsync(assetGroupId);
            if (entities != null && entities.Count > 0) { assetTypes = entities.ToList(); }
            return assetTypes;
        }

        #endregion

        //============== Asset Group Action Methods ======================//
        #region Asset Group Action Methods

        public async Task<bool> CreateAssetGroupAsync(AssetGroup assetGroup)
        {
            if (assetGroup == null) { throw new ArgumentNullException(nameof(assetGroup), "Required parameter [AssetGroup] is missing. The request cannot be processed."); }
            return await _assetGroupRepository.AddAsync(assetGroup);
        }

        public async Task<bool> DeleteAssetGroupAsync(int assetGroupId)
        {
            if (assetGroupId < 1) { throw new ArgumentNullException(nameof(assetGroupId), "Required parameter [AssetGroupID] is missing."); }
            return await _assetGroupRepository.DeleteAsync(assetGroupId);

        }

        public async Task<bool> UpdateAssetGroupAsync(AssetGroup assetGroup)
        {
            if (assetGroup == null) { throw new ArgumentNullException(nameof(assetGroup), "Required parameter [AssetGroup] is missing."); }
            return await _assetGroupRepository.EditAsync(assetGroup);
        }

        public async Task<AssetGroup> GetAssetGroupByIdAsync(int assetGroupId)
        {
            AssetGroup assetGroup = new AssetGroup();
            if (assetGroupId < 1) { throw new ArgumentNullException(nameof(assetGroupId), "The required parameter [assetGroupId] is missing. The request cannot be processed."); }
            return await _assetGroupRepository.GetByIdAsync(assetGroupId);
        }

        public async Task<IList<AssetGroup>> GetAssetGroupsByCategoryIdAsync(int assetCategoryId)
        {
            List<AssetGroup> assetGroups = new List<AssetGroup>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }

            var entities = await _assetGroupRepository.GetByCategoryIdAsync(assetCategoryId);
            if (entities != null && entities.Count > 0) { assetGroups = entities.ToList(); }

            return assetGroups;
        }

        public async Task<IList<AssetGroup>> GetAssetGroupsByClassIdAsync(int assetClassId)
        {
            List<AssetGroup> assetGroups = new List<AssetGroup>();
            if (assetClassId < 1) { throw new ArgumentNullException(nameof(assetClassId), "The required parameter [AssetClassID] is missing. The request cannot be processed."); }
            var entities = await _assetGroupRepository.GetByClassIdAsync(assetClassId);
            if (entities != null && entities.Count > 0) { assetGroups = entities.ToList(); }
            return assetGroups;
        }

        public async Task<IList<AssetGroup>> SearchAssetGroupsByNameAsync(string assetGroupName)
        {
            List<AssetGroup> assetGroups = new List<AssetGroup>();
            var entities = await _assetGroupRepository.GetByNameAsync(assetGroupName);
            if (entities != null && entities.Count > 0) { assetGroups = entities.ToList(); }
            return assetGroups;
        }

        public async Task<IList<AssetGroup>> GetAssetGroupsAsync()
        {
            List<AssetGroup> assetGroups = new List<AssetGroup>();
            var entities = await _assetGroupRepository.GetAllAsync();
            if (entities != null && entities.Count > 0) { assetGroups = entities.ToList(); }
            return assetGroups;
        }

        #endregion

        //============= Asset Bin Location Action Methods ===============//
        #region Asset Bin Location Write Action Methods

        public async Task<bool> CreateAssetBinLocationAsync(AssetBinLocation assetBinLocation)
        {
            if (assetBinLocation == null) { throw new ArgumentNullException(nameof(assetBinLocation), "Required parameter [AssetBinLocation] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetBinLocationRepository.AddAsync(assetBinLocation);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetBinLocationAsync(int assetBinLocationId)
        {
            if (assetBinLocationId < 1) { throw new ArgumentNullException(nameof(assetBinLocationId), "Required parameter [AssetBinLocationID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetBinLocationRepository.DeleteAsync(assetBinLocationId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssetBinLocationAsync(AssetBinLocation assetBinLocation)
        {
            if (assetBinLocation == null) { throw new ArgumentNullException(nameof(assetBinLocation), "Required parameter [AssetBinLocation] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetBinLocationRepository.EditAsync(assetBinLocation);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        #endregion

        #region Asset Bin Location Read Action Methods
        public async Task<IList<AssetBinLocation>> GetAssetBinLocationsAsync(string userId)
        {
            List<AssetBinLocation> assetBinLocations = new List<AssetBinLocation>();
            var entities = await _assetBinLocationRepository.GetAllAsync(userId);
            if (entities != null && entities.Count > 0) { assetBinLocations = entities.ToList(); }
            return assetBinLocations;
        }

        public async Task<AssetBinLocation> GetAssetBinLocationByIdAsync(int assetBinLocationId)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            if (assetBinLocationId < 1) { throw new ArgumentNullException(nameof(assetBinLocationId), "The required parameter [assetBinLocationId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _assetBinLocationRepository.GetByIdAsync(assetBinLocationId);
                if (entity != null) { assetBinLocation = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetBinLocation;
        }

        public async Task<AssetBinLocation> GetAssetBinLocationByNameAsync(string assetBinLocationName, string userId = null)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var entities = await _assetBinLocationRepository.GetByNameAsync(assetBinLocationName, userId);
                if (entities != null && entities.Count > 0) { assetBinLocation = entities.ToList().FirstOrDefault(); }
            }
            else
            {
                var entities = await _assetBinLocationRepository.GetByNameAsync(assetBinLocationName);
                if (entities != null && entities.Count > 0) { assetBinLocation = entities.ToList().FirstOrDefault(); }
            }

            return assetBinLocation;
        }

        public async Task<IList<AssetBinLocation>> SearchAssetBinLocationsByNameAsync(string assetBinLocationName, string userId)
        {
            List<AssetBinLocation> assetBinLocations = new List<AssetBinLocation>();
            var entities = await _assetBinLocationRepository.SearchByNameAsync(assetBinLocationName, userId);
            if (entities != null && entities.Count > 0) { assetBinLocations = entities.ToList(); }
            return assetBinLocations;
        }

        public async Task<IList<AssetBinLocation>> GetAssetBinLocationsByLocationIdAsync(int assetLocationId, string userId)
        {
            List<AssetBinLocation> assetBinLocations = new List<AssetBinLocation>();
            if (assetLocationId < 1) { throw new ArgumentNullException(nameof(assetLocationId), "The required parameter [AssetLocationID] is missing. The request cannot be processed."); }
            var entities = await _assetBinLocationRepository.GetByLocationIdAsync(assetLocationId, userId);
            if (entities != null && entities.Count > 0) { assetBinLocations = entities.ToList(); }
            return assetBinLocations;
        }
        #endregion

        //============= Asset Write Service Methods =====================//
        #region Asset Write Service Methods
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
                IsSuccessful = await _assetRepository.DeletePermanentlyAsync(assetId);
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
        #endregion

        //============= Asset Read Service Methods =======================//
        #region Asset Read Service Methods
        public async Task<IList<Asset>> GetAssetsAsync(string userId)
        {
            List<Asset> assets = new List<Asset>();
            try
            {
                var entities = await _assetRepository.GetAllAsync(userId);
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

        public async Task<IList<Asset>> SearchAssetsByNameAsync(string assetName, string userId)
        {
            List<Asset> assets = new List<Asset>();
            try
            {
                var entities = await _assetRepository.SearchByNameAsync(assetName, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByAssetTypeIdAsync(int assetTypeId, string userId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId), "The required parameter [AssetTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByAssetTypeIdAsync(assetTypeId, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByCategoryIdAsync(int assetCategoryId, string userId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetCategoryId < 1) { throw new ArgumentNullException(nameof(assetCategoryId), "The required parameter [AssetCategoryID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByCategoryIdAsync(assetCategoryId, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByClassIdAsync(int assetClassId, string userId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetClassId < 1) { throw new ArgumentNullException(nameof(assetClassId), "The required parameter [AssetClassID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByClassIdAsync(assetClassId, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByDivisionIdAsync(int assetDivisionId, string userId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetDivisionId < 1) { throw new ArgumentNullException(nameof(assetDivisionId), "The required parameter [AssetDivisionID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByDivisionIdAsync(assetDivisionId, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }

        public async Task<IList<Asset>> GetAssetsByAssetGroupIdAsync(int assetGroupId, string userId)
        {
            List<Asset> assets = new List<Asset>();
            if (assetGroupId < 1) { throw new ArgumentNullException(nameof(assetGroupId), "The required parameter [AssetGroupID] is missing. The request cannot be processed."); }
            try
            {
                var entities = await _assetRepository.GetByAssetGroupIdAsync(assetGroupId, userId);
                if (entities != null && entities.Count > 0) { assets = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assets;
        }


        #endregion

        #region Asset Report Service Methods
        public async Task<List<Asset>> GetAssetStatusReportAsync(string UserId, int? BaseLocationID = null, int? BinLocationID = null, int? AssetGroupID = null, int? AssetTypeID = null, int? AssetCondition = null)
        {
            List<Asset> AssetList = new List<Asset>();
            if(BinLocationID != null && BinLocationID.Value > 0)
            {
                if(AssetTypeID != null && AssetTypeID > 0)
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetTypeIdnAssetConditionAsync(BinLocationID.Value, AssetTypeID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetTypeIdAsync(BinLocationID.Value, AssetTypeID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else if(AssetGroupID != null && AssetGroupID > 0)
                {
                    if (AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetGroupIdnAssetConditionAsync(BinLocationID.Value, AssetGroupID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetGroupIdAsync(BinLocationID.Value, AssetGroupID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetConditionAsync(BinLocationID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdAsync(BinLocationID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
            }
            else if(BaseLocationID != null && BaseLocationID.Value > 0)
            {
                if (AssetTypeID != null && AssetTypeID > 0)
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdnAssetTypeIdnAssetConditionAsync(BaseLocationID.Value, AssetTypeID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdnAssetTypeIdAsync(BaseLocationID.Value, AssetTypeID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else if (AssetGroupID != null && AssetGroupID > 0)
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdnAssetGroupIdnAssetConditionAsync(BaseLocationID.Value, AssetGroupID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdnAssetGroupIdAsync(BaseLocationID.Value, AssetGroupID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else
                {
                    if (AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdnAssetConditionAsync(BaseLocationID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBaseLocationIdAsync(BaseLocationID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
            }
            else
            {
                if (AssetTypeID != null && AssetTypeID > 0)
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetTypeIdnAssetConditionAsync(BinLocationID.Value, AssetTypeID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetTypeIdAsync(BinLocationID.Value, AssetTypeID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else if (AssetGroupID != null && AssetGroupID > 0)
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetGroupIdnAssetConditionAsync(BinLocationID.Value, AssetGroupID.Value, AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetByBinLocationIdnAssetGroupIdAsync(BinLocationID.Value, AssetGroupID.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
                else
                {
                    if(AssetCondition != null)
                    {
                        var asset_entities = await _assetRepository.GetByAssetConditionAsync(AssetCondition.Value, UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                    else
                    {
                        var asset_entities = await _assetRepository.GetAllAsync(UserId);
                        if (asset_entities != null) { return asset_entities.ToList(); }
                    }
                }
            }
            return AssetList;
        }
        #endregion

        //============ Asset Reservation Action Methods ===================//
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
            if (reservedYear < 1) { throw new ArgumentException(nameof(reservedYear)); }
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
            if (reservedMonth < 1) { throw new ArgumentException(nameof(reservedMonth)); }
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

        //============ Asset Usage Action Methods =========================//
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

                    if (!string.IsNullOrWhiteSpace(previousLocation))
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
                if (usageYear != null && usageYear > 0)
                {
                    if (usageMonth != null && usageMonth > 0)
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
                    if (usageMonth != null & usageMonth > 0 && usageMonth < 13)
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

        //========= Asset Incident Action Methods =========================//
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
            if (incidentYear < 1) { throw new ArgumentException(nameof(incidentYear)); }

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

        //========== Asset Maintenance Action Methods ======================//
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
            if (maintenanceYear < 1) { throw new ArgumentException(nameof(maintenanceYear)); }
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
            if (maintenanceMonth < 1) { throw new ArgumentException(nameof(maintenanceMonth)); }
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

        //========== Asset Movement Action Methods =========================//
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
                    await _assetRepository.UpdateBaseLocationAsync(assetMovement.AssetID, assetMovement.MovedToLocationID.Value, assetMovement.MovedToLocationName, assetMovement.MovedToBinLocationID, assetMovement.ModifiedBy);
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
                    await _assetRepository.UpdateBaseLocationAsync(assetMovement.AssetID, assetMovement.MovedToLocationID.Value, assetMovement.MovedToLocationName, assetMovement.MovedToBinLocationID, assetMovement.ModifiedBy);
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
            if (movementYear < 1) { throw new ArgumentException(nameof(movementYear)); }
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
