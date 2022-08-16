using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetMaintenanceViewModel : BaseViewModel
    {
        public int? AssetMaintenanceID { get; set; }

        [Required]
        public string AssetID { get; set; }

        [Required]
        [Display(Name ="Equipment Name")]
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name ="Start On")]
        public DateTime? StartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Completed On")]
        public DateTime? EndTime { get; set; }

        [Required]
        [Display(Name = "Title")]
        [MaxLength(150, ErrorMessage ="Title must not exceed 150 characters.")]
        public string MaintenanceTitle { get; set; }

        [Required]
        [Display(Name = "Issue Description")]
        [MaxLength(5000, ErrorMessage = "Issue Description must not exceed 5000 characters.")]
        public string IssueDescription { get; set; }

        [Required]
        [Display(Name = "Solution Description")]
        [MaxLength(5000, ErrorMessage = "Solution Description must not exceed 5000 characters.")]
        public string SolutionDescription { get; set; }

        [Required]
        [Display(Name = "Previous Condition")]
        public string PreviousCondition { get; set; }

        [Required]
        [Display(Name = "Current Condition")]
        public string FinalCondition { get; set; }

        [Display(Name ="Executed By")]
        public string MaintainedBy { get; set; }

        [Display(Name = "Supervised By")]
        public string SupervisedBy { get; set; }

        public string Comments { get; set; }

        [Display(Name = "Logged By")]
        [MaxLength(150, ErrorMessage = "Must not exceed 150 characters.")]
        public string LoggedBy { get; set; }

        [Display(Name = "Logged On")]
        [DataType(DataType.DateTime)]
        public DateTime? LoggedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }

        public AssetMaintenance ConvertToAssetMaintenance()
        {
            return new AssetMaintenance
            {
                AssetCategoryID = AssetCategoryID,
                AssetCategoryName = AssetCategoryName,
                AssetDescription = AssetDescription,
                AssetID = AssetID,
                AssetMaintenanceID = AssetMaintenanceID == null ? 0 : AssetMaintenanceID.Value,
                AssetName = AssetName,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                Comments = Comments,
                EndTime = EndTime == null ? DateTime.Now : EndTime.Value,
                FinalCondition = FinalCondition,
                IssueDescription = IssueDescription,
                LoggedBy = LoggedBy,
                LoggedTime = LoggedTime == null ? DateTime.Now : LoggedTime.Value,
                MaintainedBy = MaintainedBy,
                MaintenanceTitle = MaintenanceTitle,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                PreviousCondition = PreviousCondition,
                SolutionDescription = SolutionDescription,
                StartTime = StartTime == null ? DateTime.Now : StartTime.Value,
                SupervisedBy = SupervisedBy,
            };
        }
    }
}
