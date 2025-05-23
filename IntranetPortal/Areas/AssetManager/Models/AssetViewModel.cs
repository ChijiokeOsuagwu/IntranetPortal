﻿using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetViewModel:BaseViewModel
    {
        public string AssetID { get; set; }

        [Required]
        [Display(Name ="Name*")]
        [MaxLength(100, ErrorMessage ="Name must not exceed 100 characters.")]
        public string AssetName { get; set; }

        [Display(Name = "Number")]
        [MaxLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
        public string AssetNumber { get; set; }

        [Display(Name = "Custom Field")]
        [MaxLength(250, ErrorMessage = "Custom No. must not exceed 250 characters.")]
        public string CustomField { get; set; }

        [Display(Name = "Asset Description")]
        [MaxLength(250, ErrorMessage = "Name must not exceed 250 characters.")]
        public string AssetDescription { get; set; }

        [Required(ErrorMessage ="Invalid Asset Type.")]
        [Display(Name = "Type*")]
        public int AssetTypeID { get; set; }

        [Display(Name = "Type*")]
        public string AssetTypeName { get; set; }

        [Required(ErrorMessage = "Invalid Asset Group.")]
        [Display(Name = "Group*")]
        public int? AssetGroupID { get; set; }

        [Display(Name = "Group*")]
        public string AssetGroupName { get; set; }

        [Required(ErrorMessage = "Invalid Asset Class.")]
        [Display(Name = "Class*")]
        public int AssetClassID { get; set; }

        [Display(Name = "Class*")]
        public string AssetClassName { get; set; }

        [Required(ErrorMessage = "Invalid Asset Category.")]
        [Display(Name ="Category")]
        public int AssetCategoryID { get; set; }

        [Display(Name ="Category")]
        public string AssetCategoryName { get; set; }

        [Required(ErrorMessage = "Please select Base Location.")]
        [Display(Name ="Base Location")]
        public int? BaseLocationID { get; set; }

        [Display(Name = "Base Location")]
        public string BaseLocationName { get; set; }

        [Display(Name = "Bin Location")]
        public int? BinLocationID { get; set; }

        [Display(Name = "Bin Location")]
        public string BinLocationName { get; set; }

        [Required(ErrorMessage = "Please select the Control Unit.")]
        [Display(Name = "Control Unit*")]
        public int AssetDivisionID { get; set; }

        [Required(ErrorMessage ="Please select Condition.")]
        [Display(Name ="Condition")]
        public AssetCondition ConditionStatus { get; set; }

        [Display(Name="Condition Description")]
        public string ConditionDescription { get; set; }

        [Display(Name ="Operational Location")]
        public string CurrentLocation { get; set; }

        [Display(Name ="Master Asset")]
        public string ParentAssetID { get; set; }

        [Display(Name ="Master Assest")]
        public string ParentAssetName { get; set; }

        [Display(Name ="Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name="Purchase Date")]
        public string PurchaseDateFormatted { get; set; }

        [Display(Name ="Purchase Amount")]
        [DataType(DataType.Currency)]
        public decimal? PurchaseAmount { get; set; }

        [Display(Name="Purchase Amount")]
        public string PurchaseAmountFormatted { get; set; }

        [Display(Name = "Usage Status")]
        public string UsageStatus { get; set; }

        public long NoOfConfirmedBooking { get; set; }
        public string ImagePath { get; set; }
        public string OldImagePath { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile ImageUpload { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public Asset ConvertToAsset()
        {
            return new Asset()
            {
                AssetCategoryID = AssetCategoryID,
                AssetCategoryName = AssetCategoryName,
                AssetClassID = AssetClassID,
                AssetClassName = AssetClassName,
                AssetGroupID = AssetGroupID,
                AssetGroupName = AssetGroupName,
                AssetDescription = AssetDescription,
                AssetDivisionID = AssetDivisionID,
                AssetID = AssetID,
                AssetName = AssetName,
                AssetNumber = AssetNumber,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                CustomField = CustomField,
                ParentAssetID = ParentAssetID,
                BaseLocationID = BaseLocationID,
                BaseLocationName = BaseLocationName,
                BinLocationID = BinLocationID,
                BinLocationName = BinLocationName,
                ConditionStatus = ConditionStatus,
                ConditionDescription = ConditionDescription,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate,
                CurrentLocation = CurrentLocation,
                ImagePath = ImagePath,
                NoOfConfirmedBooking = NoOfConfirmedBooking,
                ModifiedBy = ModifiedBy,
                ModifiedDate = ModifiedDate,
                ParentAssetName = ParentAssetName,
                PurchaseAmount = PurchaseAmount,
                PurchaseDate = PurchaseDate,
                UsageStatus = UsageStatus
            };
        }

        public AssetViewModel RevertToModel(Asset asset)
        {
            return new AssetViewModel()
            {
                AssetCategoryID = asset.AssetCategoryID,
                AssetCategoryName = asset.AssetCategoryName,
                AssetClassID = asset.AssetClassID ?? 0,
                AssetClassName = asset.AssetClassName,
                AssetDescription = asset.AssetDescription,
                AssetDivisionID = asset.AssetDivisionID ?? 0,
                AssetID = asset.AssetID,
                AssetName = asset.AssetName,
                AssetNumber = asset.AssetNumber,
                AssetTypeID = asset.AssetTypeID,
                AssetTypeName = asset.AssetTypeName,
                ParentAssetID = asset.ParentAssetID,
                BaseLocationID = asset.BaseLocationID,
                BaseLocationName = asset.BaseLocationName,
                BinLocationID = asset.BinLocationID,
                BinLocationName = asset.BinLocationName,
                ConditionDescription = asset.ConditionDescription,
                ConditionStatus = asset.ConditionStatus,
                CreatedBy = asset.CreatedBy,
                CreatedDate = asset.CreatedDate,
                CurrentLocation = asset.CurrentLocation,
                ImagePath = asset.ImagePath,
                ModifiedBy = asset.ModifiedBy,
                ModifiedDate = asset.ModifiedDate,
                OldImagePath = asset.ImagePath,
                ParentAssetName = asset.ParentAssetName,
                PurchaseAmount = asset.PurchaseAmount,
                PurchaseDate = asset.PurchaseDate,
                UsageStatus = asset.UsageStatus
            };
        }
    }
}
