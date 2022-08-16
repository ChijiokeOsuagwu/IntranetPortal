using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetIncidentViewModel : BaseViewModel
    {
        public int? AssetIncidentID { get; set; }
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
        [Display(Name ="Incident Time")]
        [DataType(DataType.DateTime)]
        public DateTime? IncidentTime { get; set; }

        [Required]
        [Display(Name ="Incident Title")]
        [MaxLength(100, ErrorMessage ="Incident Title must not exceed 100 characters.")]
        public string IncidentTitle { get; set; }

        [Required]
        [Display(Name ="Incident Description")]
        [MaxLength(5000, ErrorMessage ="Incident Description must not exceed 5000 characters.")]
        public string IncidentDescription { get; set; }

        [Required]
        [Display(Name ="Equipment Condition")]
        public string AssetCondition { get; set; }

        [Display(Name = "Action Taken")]
        [MaxLength(5000, ErrorMessage = "Action Taken must not exceed 5000 characters.")]
        public string ActionTaken { get; set; }

        [Display(Name = "Recommendation")]
        [MaxLength(5000, ErrorMessage = "Recommendation must not exceed 5000 characters.")]
        public string Recommendation { get; set; }

        [Display(Name = "Comments")]
        [MaxLength(5000, ErrorMessage = "Comments must not exceed 5000 characters.")]
        public string Comments { get; set; }
        public string LoggedBy { get; set; }
        public DateTime? LoggedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }

        public AssetIncident ConvertToAssetIncident()
        {
            return new AssetIncident
            {
                ActionTaken = ActionTaken,
                AssetCategoryID = AssetCategoryID,
                AssetCategoryName = AssetCategoryName,
                AssetCondition = AssetCondition,
                AssetDescription = AssetDescription,
                AssetID = AssetID,
                AssetIncidentID = AssetIncidentID == null ? 0 : AssetIncidentID.Value,
                AssetName = AssetName,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                Comments = Comments,
                IncidentDescription = IncidentDescription,
                IncidentTime = IncidentTime == null ? (DateTime?)null : IncidentTime.Value,
                IncidentTitle = IncidentTitle,
                LoggedBy = LoggedBy,
                LoggedTime = LoggedTime == null ? (DateTime?)null : LoggedTime.Value,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                Recommendation = Recommendation
            };
        }
    }
}
