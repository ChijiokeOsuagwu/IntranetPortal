using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetCheckInViewModel : BaseViewModel
    {
        public int UsageID { get; set; }

        [Required]
        public string AssetID { get; set; }

        [Display(Name = "Equipment Name")]
        public string AssetName { get; set; }

        public string AssetDescription { get; set; }

        [Display(Name = "Checked In By")]
        public string CheckedInBy { get; set; }

        [Display(Name = "Checked In Time")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckedInTime { get; set; }

        [Display(Name = "Condition")]
        public AssetCondition CheckedInCondition { get; set; }

        [Display(Name = "To Location")]
        [Required]
        public string CheckedInToLocation { get; set; }

        [Display(Name = "From Location")]
        public string CheckedInFromLocation { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(1500, ErrorMessage = "Comment must not exceed 1500 characters.")]
        public string CheckedInComment { get; set; }

        [Display(Name = "Status")]
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
                CheckedInBy = CheckedInBy,
                CheckedInComment = CheckedInComment,
                CheckedInCondition = CheckedInCondition,
                CheckedInTime = CheckedInTime,
                CheckStatus = CheckStatus,
                UsageID = UsageID,
                UsageLocation = CheckedInToLocation,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
            };
        }
    }
}
