using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IAssetManagerService
    {
        //==================== Asset Category Service Methods ================================================//
        #region Asset Category Service Methods
        Task<bool> CreateAssetCategoryAsync(AssetCategory assetCategory);
        Task<bool> DeleteAssetCategoryAsync(int assetCategoryId);
        Task<bool> UpdateAssetCategoryAsync(AssetCategory assetCategory);
        Task<IList<AssetCategory>> GetAssetCategoriesAsync();
        Task<AssetCategory> GetAssetCategoryByIdAsync(int assetCategoryId);
        Task<IList<AssetCategory>> SearchAssetCategoriesByNameAsync(string assetCategoryName);
        #endregion

        //==================== Asset Types Service Methods ===================================================//
        #region Asset Types Service Methods
        Task<bool> CreateAssetTypeAsync(AssetType assetType);
        Task<bool> DeleteAssetTypeAsync(int assetTypeId);
        Task<bool> UpdateAssetTypeAsync(AssetType assetType);
        Task<IList<AssetType>> GetAssetTypesAsync();
        Task<AssetType> GetAssetTypeByIdAsync(int assetTypeId);
        Task<IList<AssetType>> SearchAssetTypesByNameAsync(string assetTypeName);
        Task<IList<AssetType>> GetAssetTypesByCategoryIdAsync(int assetCategoryId);
        #endregion

        //==================== Asset Service Methods =========================================================//
        #region Assets Service Methods
        Task<bool> CreateAssetAsync(Asset asset);
        Task<bool> DeleteAssetAsync(string assetId, string deletedBy);
        Task<bool> UpdateAssetAsync(Asset asset);
        Task<IList<Asset>> GetAssetsAsync();
        Task<Asset> GetAssetByNameAsync(string assetName);
        Task<Asset> GetAssetByIdAsync(string assetId);
        Task<IList<Asset>> SearchAssetsByNameAsync(string assetName);
        Task<IList<Asset>> GetAssetsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<Asset>> GetAssetsByCategoryIdAsync(int assetCategoryId);
        #endregion

        //=================== Asset Reservation Service Methods ==============================================//
        #region Asset Reservations Service Methods
        Task<bool> CreateAssetReservationAsync(AssetReservation assetReservation);
        Task<bool> DeleteAssetReservationAsync(int assetReservationId);
        Task<bool> UpdateAssetReservationAsync(AssetReservation assetReservation);
        Task<IList<AssetReservation>> GetAssetReservationsByAssetIdAsync(string assetId);
        Task<IList<AssetReservation>> GetAssetReservationsAsync();
        Task<AssetReservation> GetAssetReservationByIdAsync(int assetReservationId);
        Task<IList<AssetReservation>> GetAssetReservationsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetReservation>> SearchAssetReservationsByAssetNameAsync(string assetName);


        Task<IList<AssetReservation>> GetCurrentAssetReservationsByAssetIdAsync(string assetId);
        Task<IList<AssetReservation>> GetCurrentAssetReservationsAsync();
        Task<IList<AssetReservation>> GetCurrentAssetReservationsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetReservation>> SearchCurrentAssetReservationsByAssetNameAsync(string assetName);

        #endregion

        //===================== Asset Usage Service Methods ==================================================//
        #region Asset Usage Service Methods
        Task<bool> CheckOutEquipmentAsync(AssetUsage assetUsage);
        Task<bool> CancelCheckOutEquipmentAsync(int assetUsageId, string assetId, string previousLocation, string previousStatus, string modifiedBy);
        Task<bool> DeleteAssetUsageAsync(int assetUsageId);
        Task<bool> UpdateAssetUsageAsync(AssetUsage assetUsage);
        Task<IList<AssetUsage>> GetAssetUsagesAsync();
        Task<AssetUsage> GetAssetUsageByIdAsync(int assetUsageId);
        Task<IList<AssetUsage>> GetAssetUsagesByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetUsage>> GetAssetUsagesByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> GetAssetUsagesCheckedOutByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> SearchAssetUsagesByAssetNameAsync(string assetName);
        Task<IList<AssetUsage>> GetCurrentAssetUsagesAsync();
        Task<IList<AssetUsage>> GetCurrentAssetUsagesByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetUsage>> GetCurrentAssetUsagesByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> SearchCurrentAssetUsagesByAssetNameAsync(string assetName);
        #endregion

        //===================== Asset Incident Service Methods ===============================================//
        #region Asset Incident Service Methods
        Task<bool> AddAssetIncidentAsync(AssetIncident assetIncident);
        Task<bool> DeleteAssetIncidentAsync(int assetIncidentId);
        Task<bool> UpdateAssetIncidentAsync(AssetIncident assetIncident);
        Task<IList<AssetIncident>> GetAssetIncidentsAsync();
        Task<AssetIncident> GetAssetIncidentByIdAsync(int assetIncidentId);
        Task<IList<AssetIncident>> GetAssetIncidentsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetIncident>> GetAssetIncidentsByAssetIdAsync(string assetId);
        Task<IList<AssetIncident>> SearchAssetIncidentsByAssetNameAsync(string assetName);
        Task<IList<AssetIncident>> GetCurrentAssetIncidentsAsync();
        Task<IList<AssetIncident>> GetCurrentAssetIncidentsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetIncident>> GetCurrentAssetIncidentsByAssetIdAsync(string assetId);
        Task<IList<AssetIncident>> SearchCurrentAssetIncidentsByAssetNameAsync(string assetName);
        #endregion

        //===================== Asset Maintenance Service Methods ===============================================//
        #region Asset Maintenance Service Methods
        Task<bool> AddAssetMaintenanceAsync(AssetMaintenance assetMaintenance);
        Task<bool> DeleteAssetMaintenanceAsync(int assetMaintenanceId);
        Task<bool> UpdateAssetMaintenanceAsync(AssetMaintenance assetMaintenance);
        Task<IList<AssetMaintenance>> GetAssetMaintenancesAsync();
        Task<AssetMaintenance> GetAssetMaintenanceByIdAsync(int assetMaintenanceId);
        Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetMaintenance>> GetAssetMaintenancesByAssetIdAsync(string assetId);
        Task<IList<AssetMaintenance>> SearchAssetMaintenancesByAssetNameAsync(string assetName);
        Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesAsync();
        Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetMaintenance>> GetCurrentAssetMaintenancesByAssetIdAsync(string assetId);
        Task<IList<AssetMaintenance>> SearchCurrentAssetMaintenancesByAssetNameAsync(string assetName);
        #endregion

        //===================== Asset Movement Service Methods ===============================================//
        #region Asset Movement Service Methods
        Task<bool> AddAssetMovementAsync(AssetMovement assetMovement);
        Task<bool> DeleteAssetMovementAsync(int assetMovementId);
        Task<bool> UpdateAssetMovementAsync(AssetMovement assetMovement);
        Task<IList<AssetMovement>> GetAssetMovementAsync();
        Task<AssetMovement> GetAssetMovementByIdAsync(int assetMovementId);
        Task<IList<AssetMovement>> GetAssetMovementsByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetMovement>> GetAssetMovementsByAssetIdAsync(string assetId);
        Task<IList<AssetMovement>> SearchAssetMovementsByAssetNameAsync(string assetName);
        #endregion
    }
}
