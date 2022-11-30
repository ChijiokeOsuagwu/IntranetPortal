using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetMovementViewModel : BaseViewModel
    {
        public int? AssetMovementID { get; set; }
        public string AssetID { get; set; }

        [Required]
        [Display(Name = "Equipment Name")]
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Transferred")]
        public DateTime? MovedOn { get; set; }

        [Required]
        [Display(Name = "Transfer From")]
        public int? MovedFromLocationID { get; set; }

        [Display(Name = "Transfer From")]
        public string MovedFromLocationName { get; set; }

        [Required]
        [Display(Name = "Transfer To")]
        public int? MovedToLocationID { get; set; }

        [Display(Name = "Transfer To")]
        public string MovedToLocationName { get; set; }

        [Display(Name = "Purpose")]
        [MaxLength(5000, ErrorMessage = "Purpose must not exceed 5000 characters.")]
        public string MovementPurpose { get; set; }

        [Required]
        [Display(Name = "Condition Status")]
        public AssetCondition AssetConditionStatus{ get; set; }

        [Display(Name="Condition Description")]
        public string AssetConditionDescription { get; set; }

        [Required]
        [Display(Name = "Supervised By")]
        public string SupervisedBy { get; set; }

        [Required]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        public string Comments { get; set; }

        [Display(Name = "Logged By")]
        [MaxLength(150, ErrorMessage = "Must not exceed 150 characters.")]
        public string LoggedBy { get; set; }

        [Display(Name = "Logged On")]
        [DataType(DataType.DateTime)]
        public DateTime? LoggedTime { get; set; }
        public string MovementStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }

        public AssetMovement ConvertToAssetMovement()
        {
            return new AssetMovement
            {
                ApprovedBy = ApprovedBy,
                AssetCategoryID = AssetCategoryID,
                AssetCategoryName = AssetCategoryName,
                AssetConditionStatus = AssetConditionStatus,
                AssetConditionDescription = AssetConditionDescription,
                AssetDescription = AssetDescription,
                AssetID = AssetID,
                AssetMovementID = AssetMovementID == null ? 0 : AssetMovementID.Value,
                AssetName = AssetName,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                Comments = Comments,
                MovedOn = MovedOn == null ? DateTime.Today : MovedOn.Value,
                MovedFromLocationID = MovedFromLocationID,
                MovedFromLocationName = MovedFromLocationName,
                MovedToLocationID = MovedToLocationID,
                MovedToLocationName = MovedToLocationName,
                MovementPurpose = MovementPurpose,
                MovementStatus = MovementStatus,
                LoggedBy = LoggedBy,
                LoggedTime = LoggedTime == null ? DateTime.Now : LoggedTime.Value,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                SupervisedBy = SupervisedBy,
            };
        }

    }
}
