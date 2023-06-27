using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class Asset
    {
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetNumber { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int? AssetClassID { get; set; }
        public string AssetClassName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }
        public int? BaseLocationID { get; set; }
        public string BaseLocationName { get; set; }
        public int? BinLocationID { get; set; }
        public string BinLocationName { get; set; }
        public string UsageStatus { get; set; }
        public string ConditionDescription { get; set; }
        public AssetCondition ConditionStatus { get; set; }
        public string CurrentLocation { get; set; }
        public string ParentAssetID { get; set; }
        public string ParentAssetName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public long NoOfConfirmedBooking { get; set; }
        public string ImagePath { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

    }
}
