using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetUsageViewModel:BaseViewModel
    {
        public int? UsageID { get; set; }

        [Required]
        [Display(Name ="Equipment")]
        public string AssetID { get; set; }

        [Display(Name ="Equipment")]
        public string AssetName { get; set; }

        public string AssetDescription { get; set; }

        [Display(Name ="Equipment Type")]
        public int? AssetTypeID { get; set; }

        [Display(Name ="Equipment Type")]
        public string AssetTypeName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name ="Starts On")]
        public DateTime? UsageStartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name ="Ends On")]
        public DateTime? UsageEndTime { get; set; }

        [MaxLength(250,ErrorMessage ="Purpose must not exceed 250 characters.")]
        [Display(Name ="Purpose")]
        [Required]
        public string Purpose { get; set; }

        [MaxLength(250, ErrorMessage = "Event Description must not exceed 250 characters.")]
        [Display(Name = "Purpose Description")]
        [Required]
        public string UsageDescription { get; set; }

        [Required]
        [Display(Name ="Location")]
        [MaxLength(250, ErrorMessage ="Location must not exceed 250 characters.")]
        public string UsageLocation { get; set; }

        [Display(Name ="Current Location")]
        [Required]
        public string CheckedOutFromLocation { get; set; }

        [Display(Name ="Checked Out By")]
        public string CheckedOutBy { get; set; }

        [Display(Name ="Checked Out Time")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckedOutTime { get; set; }

        [Display(Name = "Condition")]
        public string CheckOutCondition { get; set; }

        [Display(Name = "Assigned To")]
        [Required]
        public string CheckedOutTo { get; set; }

        [MaxLength(1500, ErrorMessage = "Comment must not exceed 1500 characters.")]
        [Display(Name = "Comment")]
        public string CheckedOutComment { get; set; }

        [Display(Name ="Checked In By")]
        public string CheckedInBy { get; set; }

        [Display(Name ="Checked In Time")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckedInTime { get; set; }

        [Display(Name = "Condition")]
        public string CheckedInCondition { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(1500, ErrorMessage = "Comment must not exceed 1500 characters.")]
        public string CheckedInComment { get; set; }

        [Display(Name ="Status")]
        public string CheckStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }

        public AssetUsage ConvertToAssetUsage()
        {
            return new AssetUsage
            {
                AssetID = AssetID,
                AssetName = AssetName,
                AssetDescription = AssetDescription,
                AssetTypeID = AssetTypeID.Value,
                AssetTypeName = AssetTypeName,
                CheckedInBy = CheckedInBy,
                CheckedInComment = CheckedInComment,
                CheckedInCondition = CheckedInCondition,
                CheckedInTime = CheckedInTime,
                CheckedOutBy = CheckedOutBy,
                CheckedOutComment = CheckedOutComment,
                CheckedOutTime = CheckedOutTime,
                CheckedOutTo = CheckedOutTo,
                CheckOutCondition = CheckOutCondition,
                CheckedOutFromLocation = CheckedOutFromLocation,
                UsageDescription = UsageDescription,
                CheckStatus = CheckStatus,
                UsageLocation = UsageLocation,
                Purpose = Purpose,
                UsageEndTime = UsageEndTime,
                UsageID = UsageID ?? 0,
                UsageStartTime = UsageStartTime,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
            };
        }
    }
}
