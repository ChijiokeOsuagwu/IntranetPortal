using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetReservationRepository
    {
        #region All Asset Reservations Methods
        Task<IList<AssetReservation>> GetAllAsync();
        Task<AssetReservation> GetByIdAsync(int assetReservationId);

        Task<IList<AssetReservation>> GetByAssetIdAsync(string assetId);

        Task<IList<AssetReservation>> GetByAssetNameAsync(string assetName);

        Task<IList<AssetReservation>> GetByAssetTypeIdAsync(int assetTypeId);
#endregion

        #region Current Asset Reservations Methods
        Task<IList<AssetReservation>> GetAllCurrentAsync();
        Task<IList<AssetReservation>> GetCurrentByAssetIdAsync(string assetId);
        Task<IList<AssetReservation>> GetCurrentByAssetNameAsync(string assetName);
        Task<IList<AssetReservation>> GetCurrentByAssetTypeIdAsync(int assetTypeId);
        #endregion

        #region Asset Reservations CRUD Methods
        Task<bool> AddAsync(AssetReservation assetReservation);

        Task<bool> EditAsync(AssetReservation assetReservation);

        Task<bool> DeleteAsync(int assetReservationId);
        #endregion
    }
}
